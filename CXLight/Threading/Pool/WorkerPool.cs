namespace CXLight.Threading.Pool
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using DataStructures.LockingQueue;
    using CXLight.Exts;

    /// <summary>
    /// FIFO Worker thread pool.
    /// </summary>
    public class WorkerPool
    {
        // Internal MRES handling
        private readonly ManualResetEventSlim _queueIsEmptyEvent = new ManualResetEventSlim();
        private readonly ManualResetEventSlim _workerSleepingEvent = new ManualResetEventSlim(false);

        // Workers
        private readonly List<PoolWorker> _poolWorkers = new List<PoolWorker>();
        private readonly ConcurrentStack<PoolWorker> _sleepingStack = new ConcurrentStack<PoolWorker>();

        // Cancellation
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationToken;

        // Queue & Pooling Thread
        private readonly LockingQueue<Action> _jobQueue = new LockingQueue<Action>();
        private Thread _poolThread;

        public WorkerPool(int count = -1)
        {
            _cancellationToken = _cancellationTokenSource.Token;
            var possiblyOptimalWorkers = (int)Math.Ceiling(Environment.ProcessorCount * 0.618);

            SpawnInitialWorkers(count <= 0 ? possiblyOptimalWorkers : count);
            SpinWaitUntilWorkersSpawned();
        }

        #region Worker handling

        private PoolWorker GetPoolWorker(int id)
        {
            var poolWorker = new PoolWorker();

            poolWorker.Thread = new Thread(() => AssignJob(poolWorker));
            poolWorker.Name = $"Worker #{id}";

            return poolWorker;
        }

        private void SpawnWorker()
        {
            var poolWorker = GetPoolWorker(_poolWorkers.Count);

            _poolWorkers.Add(poolWorker);

            poolWorker.Init();
        }

        private void SpawnInitialWorkers(int count)
        {
            var i = 0;
            while (i < count)
            {
                SpawnWorker();
                i++;
            }
        }

        #endregion

        #region Worker Assignment

        private void SignalWorkerAwakeStatus()
        {
            if (_sleepingStack.Count == 0) _workerSleepingEvent.Reset();
        }

        private bool WaitingForSleepingWorker()
        {
            return _workerSleepingEvent.TryWait(_cancellationToken);
        }

        private bool TryToWakeWorker()
        {
            if (!_sleepingStack.TryPop(out var worker)) return false;
            worker.Awake();

            return true;
        }

        private bool WakeWaitingWorker()
        {
            if (!WaitingForSleepingWorker()) return false;
            if (!TryToWakeWorker()) return false;

            SignalWorkerAwakeStatus();

            return true;
        }

        private bool WaitingForJob()
        {
            return _queueIsEmptyEvent.TryWait(_cancellationToken);
        }

        private void AssignWorkers()
        {
            while (WaitingForJob())
            {
                if (!WakeWaitingWorker())
                {
                    // Do something if no workers available
                }
            }
        }

        #endregion

        #region Job Assignment

        private void SignalJobQueueStatus()
        {
            if (_jobQueue.Count == 0) { _queueIsEmptyEvent.Reset(); }
        }

        private void SignalJobIsReady()
        {
            _queueIsEmptyEvent.Set();
        }

        private void SignalWorkerAsSleeping()
        {
            _workerSleepingEvent.Set();
        }

        private void SendWorkerToSleep(PoolWorker poolWorker)
        {
            _sleepingStack.Push(poolWorker);
            SignalWorkerAsSleeping();
        }

        private void DoJob()
        {
            _jobQueue.Dequeue()?.Invoke();
        }

        private void AssignJob(PoolWorker poolWorker)
        {
            poolWorker.IsSpawned = true;
            SendWorkerToSleep(poolWorker);

            while (poolWorker.SleepUntilJob(_cancellationToken))
            {
                DoJob();
                SignalJobQueueStatus();

                poolWorker.SendToSleep();
                SendWorkerToSleep(poolWorker);
            }
        }

        #endregion

        #region Task & Flow Handling

        public void AddJob(Action action)
        {
            _jobQueue.Enqueue(action);
            SignalJobIsReady();
        }

        public void ClearJobs()
        {
            while (_jobQueue.Count > 0) _jobQueue.Dequeue();
        }

        public void Start()
        {
            _poolThread = new Thread(AssignWorkers) { Name = "Worker Pool Main Thread" };
            _poolThread.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        #endregion

        #region Spin Waits

        public void SpinWaitUntilWorkersSpawned()
        {
            while (!AllSpawned) { }
        }

        public void SpinWaitUntilComplete()
        {
            while (!IsJobQueueEmpty) { }
            // While the job queue might be empty, some workers might still be at it
            while (!AreSleeping) { }
        }

        public void SpinWaitUntilReady()
        {
            while (!IsAlive) { }
        }

        public void SpinWaitUntilDead()
        {
            while (IsAlive) { }
        }

        #endregion

        #region Status

        private bool AreSleeping
        {
            get { return _poolWorkers.All(worker => worker.IsSleeping); }
        }

        public bool AreSpawned
        {
            get { return _poolWorkers.All(worker => worker.IsSpawned); }
        }

        public bool IsAlive => _poolThread.IsAlive;

        public bool IsJobQueueEmpty => _jobQueue.IsEmpty;

        public bool AllSpawned => AreSpawned;

        #endregion
    }
}