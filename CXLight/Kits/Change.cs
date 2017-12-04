namespace CXLight.Kits
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A conversion tool. Named change as to not conflict with .NET's Convert.
    /// </summary>
    public static class Change
    {
        public static List<Tuple<T1, T2, T3>> TupleOfListsToListOfTuples<T1, T2, T3>(Tuple<List<T1>, List<T2>, List<T3>> tupleOfLists)
        {
            if (tupleOfLists.Item1.Count != tupleOfLists.Item2.Count && tupleOfLists.Item2.Count != tupleOfLists.Item3.Count) return null;

            var result = new List<Tuple<T1, T2, T3>>();
            var c = tupleOfLists.Item1.Count;
            var i = 0;
            while (i < c)
            {
                result.Add(Tuple.Create(tupleOfLists.Item1[i], tupleOfLists.Item2[i], tupleOfLists.Item3[i]));
                i++;
            }

            return result;
        }

        #region Time related

        public static double TicksToMs(long ticks)
        {
            return ticks / 10000.0;
        }

        public static double TicksToS(long ticks)
        {
            return ticks / 10000000.0;
        }

        #endregion

        #region Space related

        public static float BytesToMb(float bytes)
        {
            return bytes / 1048576;
        }

        public static double BytesToMb(int bytes)
        {
            return bytes / 1048576.0;
        }

        #endregion
    }
}