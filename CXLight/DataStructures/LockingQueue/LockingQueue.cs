namespace CXLight.DataStructures.LockingQueue
{
    using System.Collections.Generic;

    public class LockingQueue<T1>
    {
        private readonly Queue<T1> _queue = new Queue<T1>();
        private readonly object _lock = new object();

        public T1 Dequeue()
        {
            lock (_lock)
            {
                return _queue.Count > 0 ? _queue.Dequeue() : default(T1);
            }
        }

        public void Enqueue(T1 action)
        {
            lock (_lock) { _queue.Enqueue(action); }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count;
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count == 0;
                }
            }
        }
    }
}