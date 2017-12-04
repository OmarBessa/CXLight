namespace CXLight.Threading.Pool
{
    using System.Threading;
    using CXLight.Exts;

    public class PoolWorker
    {
        private readonly ManualResetEventSlim _mres;

        public Thread Thread;

        public PoolWorker()
        {
            _mres = new ManualResetEventSlim();
            IsSpawned = false;
        }

        public bool IsSpawned { get; set; }

        public string Name
        {
            get => Thread != null ? Thread.Name : string.Empty;
            set { if (Thread != null) { Thread.Name = value; } }
        }

        public bool SleepUntilJob(CancellationToken cancellationToken)
        {
            return _mres.TryWait(cancellationToken);
        }

        public void SendToSleep()
        {
            _mres.Reset();
        }

        public void Awake()
        {
            _mres.Set();
        }

        public void Init()
        {
            _mres.Reset();
            Thread.Start();
        }

        public bool IsSleeping => !_mres.IsSet;
    }
}