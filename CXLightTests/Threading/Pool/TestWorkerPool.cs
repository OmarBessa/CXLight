namespace CXLightTests.Threading.Pool
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using CXLight.Exts;
    using CXLight.Threading.Pool;
    using CXLight.Threading.Pool.Exts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestWorkerPool
    {
        [TestMethod]
        public void TestStartTime()
        {
            var pool = new WorkerPool();

            var sw = new Stopwatch();
            sw.Start();

            pool.Start();
            pool.SpinWaitUntilWorkersSpawned();

            sw.Stop();

            Debug.WriteLine("Start time (ns): " + sw.ElapsedTicks * 100);

            Assert.IsTrue(pool.AreSpawned);
        }

        [TestMethod]
        public void TestStopTime()
        {
            var pool = new WorkerPool();

            var sw = new Stopwatch();
            pool.Start();
            while (!pool.AllSpawned) { }

            sw.Start();
            pool.Stop();
            while (pool.IsAlive) { }
            sw.Stop();

            Debug.WriteLine("Stop time (ns): " + sw.ElapsedTicks * 100);

            Assert.IsFalse(pool.IsAlive);
        }

        [TestMethod]
        public void TestExitWhileEmpty()
        {
            var pool = new WorkerPool();

            var sw = new Stopwatch();
            sw.Start();
            pool.Start();
            pool.SpinWaitUntilReady();
            pool.Stop();
            pool.SpinWaitUntilDead();
            sw.Stop();

            Debug.WriteLine("Empty pool (ns): " + sw.ElapsedNs());

            Assert.IsFalse(pool.IsAlive);
        }

        [TestMethod]
        public void TestConsistency()
        {
            const int amount = 1000;

            var pool = new WorkerPool();

            var c = new Dictionary<string, int> { ["count"] = 0 };
            pool.AddLockedTasks(() => { c["count"]++; }, amount);

            var sw = new Stopwatch();

            pool.Start();

            sw.Start();
            pool.SpinWaitUntilComplete();
            sw.Stop();

            Debug.WriteLine("Multi-threaded Ticks Init (ns): " + sw.ElapsedTicks * 100);

            Assert.IsTrue(c["count"] == amount);
        }

        [TestMethod]
        public void TestFindCurve()
        {
            void ThreadThing(WorkerPool pool)
            {
                const int amount = 10;

                var c = new Dictionary<string, int> { ["count"] = 0 };

                pool.AddLockedTasks(() => { c["count"]++; }, amount);
                pool.SpinWaitUntilComplete();
            }

            var threadCount = Environment.ProcessorCount;
            const int samples = 10;

            var i = 1;
            while (i < threadCount)
            {
                var sw = new Stopwatch();
                var pool = new WorkerPool(i);

                pool.Start();

                sw.Start();
                var j = 0;
                while (j < samples)
                {
                    ThreadThing(pool);
                    j++;
                }
                sw.Stop();

                pool.Stop();

                Debug.WriteLine($"{i}," + sw.ElapsedNs() / samples);

                i++;
            }

            Console.WriteLine("");
        }

        [TestMethod]
        public void TestCalculateOverhead()
        {
            const int amount = 1000;

            var pool = new WorkerPool(15);
            var c = new Dictionary<string, int> { ["count"] = 0 };
            pool.AddLockedTasks(() => { c["count"]++; }, amount);

            var sw = new Stopwatch();

            pool.Start();

            sw.Start();
            while (!pool.IsJobQueueEmpty) { }
            sw.Stop();

            Debug.WriteLine("Multi-threaded Ticks Init (ns): " + sw.ElapsedNs());

            pool.AddLockedTasks(() => { c["count"]++; }, amount);

            sw.Restart();
            while (!pool.IsJobQueueEmpty) { }
            sw.Stop();

            Debug.WriteLine("Multi-threaded Ticks Spun (ns): " + sw.ElapsedNs());

            var x = new Dictionary<string, int> { ["count"] = 0 };

            sw.Restart();
            var i = 0;
            while (i < amount)
            {
                x["count"]++;
                i++;
            }
            sw.Stop();

            Debug.WriteLine("Single-threaded Ticks (ns): " + sw.ElapsedTicks * 100);
        }

        [TestMethod]
        public void TestCalculateOverhead1Ms()
        {
            const int amount = 1000;

            var pool = new WorkerPool();
            pool.AddTasks(() => { Thread.Sleep(1); }, amount);

            var sw = new Stopwatch();

            pool.Start();

            sw.Start();
            while (!pool.IsJobQueueEmpty) { }
            sw.Stop();

            var multiMs = sw.ElapsedMilliseconds;

            Debug.WriteLine("Multi-threaded (ms): " + multiMs);

            sw.Restart();
            var i = 0;
            while (i < amount)
            {
                Thread.Sleep(1);
                i++;
            }
            sw.Stop();

            var singleMs = sw.ElapsedMilliseconds;

            Debug.WriteLine("Single-threaded (ms): " + singleMs);

            Assert.IsTrue(multiMs < singleMs);
        }

        [TestMethod]
        public void TestMergeSort()
        {
            List<int> GetRandomList(int amount)
            {
                var rng = new Random();

                var result = new List<int>();
                var i = 0;
                while (i < amount)
                {
                    result.Add(rng.Next(0, amount * 10));
                    i++;
                }

                return result;
            }

            List<List<int>> Divide(List<int> list)
            {
                if (list.Count == 1) return new List<List<int>> { new List<int> { list[0] } };
                if (list.Count == 2) return new List<List<int>> { new List<int> { list[0] }, new List<int> { list[1] } };

                var l = (int)Math.Ceiling(list.Count / 2.0);

                var resultA = new List<int>();
                var i = 0;
                while (i < l)
                {
                    resultA.Add(list[i]);
                    i++;
                }

                var resultB = new List<int>();
                while (i < list.Count)
                {
                    resultB.Add(list[i]);
                    i++;
                }

                return new List<List<int>> { resultA, resultB };
            }

            List<int> Merge(List<int> listA, List<int> listB)
            {
                if (listB == null) return listA;
                if (listA.Count == 1 && listB.Count == 1) return !(listA[0] > listB[0]) ? new List<int> { listA[0], listB[0] } : new List<int> { listB[0], listA[0] };

                var result = new List<int>();

                var i = 0;
                while (i < listA.Count)
                {
                    result.Add(listA[i]);
                    i++;
                }

                i = 0;
                while (i < listB.Count)
                {
                    result.Add(listB[i]);
                    i++;
                }

                result.Sort();

                return result;
            }

            bool IsOrdered(List<int> source)
            {
                var j = 1;
                while (j < source.Count)
                {
                    if (!(source[j - 1] <= source[j]))
                    {
                        return false;
                    }

                    j++;
                }

                return true;
            }

            const int size = 5;
            var listToSort = GetRandomList(size);

            var pool = new WorkerPool();
            pool.Start();

            // 1. Reduce list to least comparable size list            
            var halving = pool.Unfold(listToSort, Divide, x => x.All(y => y.Count == 1));

            // 2. Sort list back up
            var merged = pool.Fold(halving, Merge);

            Assert.IsTrue(IsOrdered(merged[0]));
        }

        [TestMethod]
        public void TestFold()
        {
            var listofeight = new List<int>();
            var i = 0;
            while (i < 8)
            {
                listofeight.Add(i + 1);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var resultA = pool.Fold(listofeight, (a, b) => a + b);

            Assert.IsTrue(resultA[0] == 36);

            var listofeleven = new List<int>();
            i = 0;
            while (i < 11)
            {
                listofeleven.Add(i + 1);
                i++;
            }

            var resultB = pool.Fold(listofeleven, (a, b) => a + b);

            Assert.IsTrue(resultB[0] == 66);
        }

        [TestMethod]
        public void TestUnfold()
        {
            var listofeight = new List<int>();
            var i = 0;
            while (i < 8)
            {
                listofeight.Add(i + 1);
                i++;
            }

            var sum = 0;
            i = 0;
            while (i < listofeight.Count)
            {
                sum += listofeight[i];
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var resultA = pool.Unfold(listofeight, a => new List<double> { a / 2.0, a / 2.0 });

            var sumA = 0.0;
            i = 0;
            while (i < resultA.Count)
            {
                sumA += resultA[i];
                i++;
            }

            Assert.IsTrue(sum == sumA);
        }

        [TestMethod]
        public void TestMap()
        {
            var listoften = new List<int>();
            var i = 0;
            while (i < 10)
            {
                listoften.Add(i + 1);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var listofdouble = pool.Map(listoften.ToArray(), x => x * 2);

            i = 0;
            while (i < listoften.Count)
            {
                Assert.IsTrue(listoften[i] * 2 == listofdouble[i]);
                i++;
            }
        }

        [TestMethod]
        public void TestFilter()
        {
            var listoften = new List<int>();
            var i = 0;
            while (i < 10)
            {
                listoften.Add(i);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var listofhalf = pool.Filter(listoften.ToArray(), x => x < 5);

            i = 0;
            while (i < listofhalf.Count)
            {
                Assert.IsTrue(listofhalf[i] < 5);
                i++;
            }
        }

        [TestMethod]
        public void TestSome()
        {
            var listoften = new List<int>();
            var i = 0;
            while (i < 10)
            {
                listoften.Add(i);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var equalsThree = pool.Some(listoften, x => x == 3);

            Assert.IsTrue(equalsThree);
        }

        [TestMethod]
        public void TestAllTrue()
        {
            var listoften = new List<int>();
            var i = 0;
            while (i < 10)
            {
                listoften.Add(i);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var areUnderEleven = pool.All(listoften, x => x < 11);

            Assert.IsTrue(areUnderEleven);
        }

        [TestMethod]
        public void TestAllFalse()
        {
            var listoften = new List<int>();
            var i = 0;
            while (i < 10)
            {
                listoften.Add(i);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var areUnderThree = pool.All(listoften, x => x < 3);

            Assert.IsTrue(!areUnderThree);
        }

        [TestMethod]
        public void TestAllTrueAndFalse()
        {
            var listoften = new List<int>();
            var i = 0;
            while (i < 10)
            {
                listoften.Add(i);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var areUnderEleven = pool.All(listoften, x => x < 11);

            Assert.IsTrue(areUnderEleven);

            var areUnderEight = pool.All(listoften, x => x < 8);

            Assert.IsFalse(areUnderEight);
        }

        [TestMethod]
        public void TestZip()
        {
            var listoften = new List<int>();
            var i = 0;
            while (i < 10)
            {
                listoften.Add(i);
                i++;
            }

            var listofdouble = new List<int>();
            i = 0;
            while (i < 10)
            {
                listofdouble.Add(i * 2);
                i++;
            }

            var pool = new WorkerPool();
            pool.Start();

            var zipped = pool.Zip(Tuple.Create(listoften, listofdouble));

            foreach (var zip in zipped)
            {
                Assert.IsTrue(zip.Item1 * 2 == zip.Item2);
            }
        }
    }
}