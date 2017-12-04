namespace CXLight.Threading.Pool.Exts
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public static class WorkerPoolExt
    {
        /// <summary>
        /// Adds a batch of identical tasks and puts a lock on each one of them. Useful for shared-data update scenarios.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="task"></param>
        /// <param name="total"></param>
        public static void AddLockedTasks(this WorkerPool pool, Action task, int total)
        {
            var taskLock = new object();

            void LockedTask()
            {
                lock (taskLock)
                {
                    task();
                }
            }

            var i = 0;
            while (i < total)
            {
                pool.AddJob(LockedTask);
                i++;
            }
        }

        /// <summary>
        /// Adds a batch of identical tasks.
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="task"></param>
        /// <param name="total"></param>
        public static void AddTasks(this WorkerPool pool, Action task, int total)
        {
            var i = 0;
            while (i < total)
            {
                pool.AddJob(task);
                i++;
            }
        }

        /// <summary>
        /// Functional Map. Now parallel!
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static T2[] Map<T1, T2>(this WorkerPool pool, T1[] source, Func<T1, T2> fn)
        {
            var result = new T2[source.Length];

            var j = 0;
            while (j < source.Length)
            {
                var j1 = j;
                pool.AddJob(() => result[j1] = fn(source[j1]));
                j++;
            }

            pool.SpinWaitUntilComplete();

            return result;
        }

        /// <summary>
        /// Functional Filter. Now parallel!
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static List<T1> Filter<T1>(this WorkerPool pool, T1[] source, Func<T1, bool> fn)
        {
            var bitMask = new bool[source.Length];

            var j = 0;
            while (j < source.Length)
            {
                var j1 = j;
                pool.AddJob(() =>
                {
                    if (fn(source[j1])) // The expensive function that makes us parallelize
                    {
                        bitMask[j1] = true;
                    }
                });

                j++;
            }

            pool.SpinWaitUntilComplete();

            // Clean list. This would be much faster by joining a Linked List instead of filtering a bit mask.
            var result = new List<T1>();
            j = 0;
            while (j < bitMask.Length)
            {
                if (bitMask[j]) { result.Add(source[j]); }
                j++;
            }

            return result;
        }

        public static List<T1> Fold<T1>(this WorkerPool pool, ConcurrentBag<T1> source, Func<T1, T1, T1> fn)
        {
            var result = new ConcurrentBag<T1>();

            if (source.Count == 1)
            {
                if (source.TryTake(out var thing))
                {
                    result.Add(fn(thing, default(T1)));
                }
            }
            else if (source.Count > 1)
            {
                if (source.Count % 2 == 0)
                {
                    var firstItem = default(T1);
                    foreach (var item in source)
                    {
                        if (firstItem == null || firstItem.Equals(default(T1)))
                        {
                            firstItem = item;
                        }
                        else
                        {
                            var item1 = firstItem;
                            var item2 = item;
                            firstItem = default(T1);

                            pool.AddJob(() => { result.Add(fn(item1, item2)); });
                        }
                    }

                    pool.SpinWaitUntilComplete();

                    return Fold(pool, result, fn);
                }

                source.Add(default(T1));
                return Fold(pool, source, fn);
            }

            return result.ToList();
        }

        /// <summary>
        /// Functional "Fold". Now parallel! 
        /// 
        /// WARNING: Use only with commutative functions. Also, fn must be defined in such a way that f(a,default) = a.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static List<T1> Fold<T1>(this WorkerPool pool, List<T1> source, Func<T1, T1, T1> fn)
        {
            var result = new ConcurrentBag<T1>();

            if (source.Count == 1)
            {
                result.Add(fn(source[0], default(T1)));

            }
            else if (source.Count > 1)
            {

                if (source.Count % 2 == 0)
                {
                    var j = 0;
                    while (j < source.Count)
                    {
                        var j1 = j;
                        pool.AddJob(() => { result.Add(fn(source[j1], source[j1 + 1])); });

                        j += 2;
                    }

                    pool.SpinWaitUntilComplete();

                    return Fold(pool, result, fn);
                }

                source.Add(default(T1));
                return Fold(pool, source, fn);
            }

            return result.ToList();
        }

        /// <summary>
        /// Functional Unfold. Now parallel! 
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static List<T2> Unfold<T1, T2>(this WorkerPool pool, List<T1> source, Func<T1, List<T2>> fn)
        {
            var results = new ConcurrentBag<List<T2>>();
            var result = new List<T2>();

            var i = 0;
            while (i < source.Count)
            {
                var i1 = i;
                pool.AddJob(() => { results.Add(fn(source[i1])); });
                i++;
            }

            pool.SpinWaitUntilComplete();

            foreach (var item in results)
            {
                result.AddRange(item);
            }

            return result;
        }

        /// <summary>
        /// Chained unfold. Useful for recursive things. Stops when the "until" function returns true.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public static List<T1> Unfold<T1>(
            this WorkerPool pool,
            T1 source,
            Func<T1, List<T1>> fn,
            Func<List<T1>, bool> until)
        {
            var cursor = pool.Unfold(new List<T1> { source }, fn);
            while (!until(cursor))
            {
                cursor = pool.Unfold(cursor, fn);
            }

            return cursor;
        }

        /// <summary>
        /// Functional zip. Now in parallel! (worthless and inefficient)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<Tuple<T1, T2>> Zip<T1, T2>(this WorkerPool pool, Tuple<List<T1>, List<T2>> source)
        {
            var result = new List<Tuple<T1, T2>>();

            var l = source.Item1.Count;
            var i = 0;
            while (i < l)
            {
                var i1 = i;
                pool.AddJob(() => result.Add(Tuple.Create(source.Item1[i1], source.Item2[i1])));
                i++;
            }

            pool.SpinWaitUntilComplete();

            return result;
        }

        /// <summary>
        /// Functional All. Now in parallel!
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static bool All<T1>(this WorkerPool pool, List<T1> source, Func<T1, bool> fn)
        {
            var negativeFound = false;

            var j = 0;
            while (j < source.Count)
            {
                var j1 = j;
                pool.AddJob(() =>
                {
                    if (negativeFound) return;
                    if (fn(source[j1])) return;

                    negativeFound = true;

                    pool.ClearJobs();
                });

                j++;
            }

            pool.SpinWaitUntilComplete();

            return !negativeFound;
        }

        /// <summary>
        /// Functional Some. Now in parallel!
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="pool"></param>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static bool Some<T1>(this WorkerPool pool, List<T1> source, Func<T1, bool> fn)
        {
            var wasFound = false;

            var j = 0;
            while (j < source.Count)
            {
                var j1 = j;
                pool.AddJob(() =>
                {
                    if (wasFound) return;
                    if (!fn(source[j1])) return;

                    wasFound = true;
                    pool.ClearJobs();
                });

                j++;
            }

            pool.SpinWaitUntilComplete();

            return wasFound;
        }
    }
}