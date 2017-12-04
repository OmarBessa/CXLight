namespace CXLightTests.DataStructures.MultiMap
{
    using System;
    using System.Collections.Generic;
    using CXLight.DataStructures.MultiMap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MultiMapTest
    {
        [TestMethod]
        public void TestAddAndCount()
        {
            var multi = new MultiMap<string, int> {{"coso", 1}, {"coso", 2}, {"coso", 3}};

            Assert.IsTrue(multi.Count == 3);
        }

        [TestMethod]
        public void TestAddWithInitializerAndCount()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            Assert.IsTrue(multi.Count == 3);
        }

        [TestMethod]
        public void TestEnumerating()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            var keys = new List<string>();
            var values = new List<int>();
            foreach (var pair in multi)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }

            Assert.IsTrue(keys[0] == "coso");
            Assert.IsTrue(keys[1] == "coso");
            Assert.IsTrue(keys[2] == "coso");

            Assert.IsTrue(values[0] == 1);
            Assert.IsTrue(values[1] == 2);
            Assert.IsTrue(values[2] == 3);
        }

        [TestMethod]
        public void TestClear()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };
            multi.Clear();

            Assert.IsTrue(multi.Count == 0);
        }

        [TestMethod]
        public void TestContains()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };
            Assert.IsTrue(multi.Contains(new KeyValuePair<string, int>("coso", 1)));
            Assert.IsFalse(multi.Contains(new KeyValuePair<string, int>("coso", 5)));

            Assert.IsFalse(new MultiMap<string, int>().Contains(new KeyValuePair<string, int>("coso", 1)));
        }

        [TestMethod]
        public void TestCopyToArrayInFull()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            var array = new KeyValuePair<string, int>[3];

            multi.CopyTo(array, 0);

            Assert.IsTrue(array.Length == multi.Count);
            Assert.IsTrue(array.Length == 3);

            var i = 0;
            while (i < array.Length)
            {
                Assert.IsTrue(array[i].Key == "coso");
                Assert.IsTrue(array[i].Value == i + 1);
                i++;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopyToArrayPartial()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            var array = new KeyValuePair<string, int>[3];

            multi.CopyTo(array, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyToArrayNull()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            multi.CopyTo(null, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToArrayOutOfRange()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };
            var array = new KeyValuePair<string, int>[3];

            multi.CopyTo(array, -1);
        }

        [TestMethod]
        public void TestRemoveByKeyValuePair()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            Assert.IsTrue(multi.Remove(new KeyValuePair<string, int>("coso", 1)));
            Assert.IsTrue(multi.Count == 2);
            Assert.IsFalse(multi.Remove(new KeyValuePair<string, int>("coso", 5)));
            Assert.IsFalse(multi.Remove(new KeyValuePair<string, int>("cosos", 2)));
        }

        [TestMethod]
        public void TestContainsKey()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            Assert.IsTrue(multi.ContainsKey("coso"));
        }

        [TestMethod]
        public void TestRemove()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            Assert.IsTrue(multi.Remove("coso"));
            Assert.IsTrue(multi.Count == 0);

            var multi2 = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 }, { "cosa", 5 } };

            Assert.IsTrue(multi2.Remove("coso"));
            Assert.IsTrue(multi2.Count == 1);
        }

        [TestMethod]
        public void TestTryGetValue()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            var didIt = multi.TryGetValue("coso", out var result);

            Assert.IsTrue(result == 1);
            Assert.IsTrue(didIt);

            var didnt = multi.TryGetValue("cosa", out var result2);

            Assert.IsTrue(result2 == 0);
            Assert.IsFalse(didnt);
        }

        [TestMethod]
        public void TestIndexical()
        {
            var multi = new MultiMap<string, int> { { "coso", 1 }, { "coso", 2 }, { "coso", 3 } };

            Assert.IsTrue(multi["coso"] == 1);

            multi["coso"] = 4;

            Assert.IsTrue(multi.Count == 4);
        }
    }
}
