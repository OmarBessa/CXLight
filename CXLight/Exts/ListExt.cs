namespace CXLight.Exts
{
    using System;
    using System.Collections.Generic;
    using Kits;

    public static class ListExt
    {
        /// <summary>
        /// Generator mapping on list container. Similar to mapping an integer array but without the memory cost.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="counter"></param>
        /// <returns></returns>
        public static List<T1> Fill<T1>(this List<T1> source, Func<int, T1> fn, int from, int to, Func<int, int> counter = null)
        {
            var i = from;

            if (from > to)
            {
                i = to;
                to = from;
            }

            if (counter == null)
            {
                while (i < to)
                {
                    source.Add(fn(i));
                    i++;
                }
            }
            else
            {
                while (i < to)
                {
                    source.Add(fn(i));
                    i = counter(i);
                }
            }

            return source;
        }

        public static List<T1> FillRandom<T1>(this List<T1> source, Func<int, T1> fn, int from, int to)
        {
            // Design 1: Map the range to a data structure, eliminate the ones who are already in the lottery. Saves random time. Might not be suitable for large collections.
            // Design 2: Continuously RNG, discard what has already showed up. Saves memory. Wastes randomness.                        
            // Design 3: Discard objects from the real collection (or a copy), remove and rng again. Rinse and repeat.

            var collisionSet = new HashSet<int>();
            var distance = to - from >= 0 ? to - from : from - to;

            while (collisionSet.Count < distance)
            {
                var dice = RngKit.Next(from, to + 1);
                if (collisionSet.Contains(dice)) continue;

                collisionSet.Add(dice);
                source.Add(fn(dice));
            }

            return source;
        }

        /// <summary>
        /// Pure functional every. Returns true if every element's predicate is true.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Every<T1>(this List<T1> source, Func<T1, bool> predicate)
        {
            var i = 0;
            while (i < source.Count)
            {
                if (!predicate(source[i])) return false;
                i++;
            }

            return true;
        }

        /// <summary>
        /// Pure functional filter. Retrieves a subset of a source set.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T1> Filter<T1>(this List<T1> source, Func<T1, bool> predicate)
        {
            var result = new List<T1>();
            var i = 0;
            while (i < source.Count)
            {
                if (predicate(source[i])) result.Add(source[i]);
                i++;
            }

            return result;
        }

        /// <summary>
        /// Filter short-hand for filtering on ascending order, element-by-element, count-based sample size.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="sampleSize"></param>
        /// <returns></returns>
        public static List<T1> Filter<T1>(this List<T1> source, int sampleSize)
        {
            var i = 0;
            return source.Filter(item => i++ < sampleSize);
        }

        /// <summary>
        /// Traversing a set without updating. Like map but without creating data.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        public static void Walk<T1>(this List<T1> source, Action<T1> fn)
        {
            var i = 0;
            while (i < source.Count)
            {
                fn(source[i]);
                i++;
            }
        }

        #region Random

        public static T PickRandomElement<T>(this IList<T> set)
        {
            return set[RngKit.Next(set.Count)];
        }

        public static T PickRandomElement<T>(this IList<T> set, int min)
        {
            return set[RngKit.Next(min, set.Count)];
        }

        public static int PickRandomIndex<T>(this IList<T> set)
        {
            return RngKit.Next(set.Count);
        }

        #endregion

        #region Interval Handling

        public static List<T> RemoveInterval<T>(this List<T> source, int start, int end)
        {
            if (start > end) return source;
            if (end > source.Count) end = source.Count;
            if (start < 0) start = 0;

            var result = new List<T>();

            var i = 0;
            while (i < start)
            {
                result.Add(source[i]);
                i++;
            }

            i = end;
            while (i < source.Count)
            {
                result.Add(source[i]);
                i++;
            }

            return result;
        }

        public static IList<T> InsertInterval<T>(this IList<T> source, IList<T> values, int at)
        {
            if (at > source.Count)
            {
                at = source.Count;
            } else if (at < 0) {
                at = 0;
            }

            var result = new List<T>();

            var i = 0;
            while (i < at)
            {
                result.Add(source[i]);
                i++;
            }

            var t = i;
            i = 0;
            while (i < values.Count)
            {
                result.Add(values[i]);
                i++;
            }

            i = t;
            while (i < source.Count)
            {
                result.Add(source[i]);
                i++;
            }

            return result;
        }

        #endregion

        #region Math-related

        /// <summary>
        /// Self-explanatory
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long Sum(this List<long> source)
        {
            var result = 0L;
            var i = 0;
            while (i < source.Count)
            {
                result += source[i];
                i++;
            }

            return result;
        }

        /// <summary>
        /// Self-explanatory
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Avg<T1>(this List<T1> source, Func<T1, double> map)
        {
            var sum = 0.0;

            var i = 0;
            while (i < source.Count)
            {
                sum += map(source[i]);
                i++;
            }

            return sum / source.Count;
        }

        public static double Max(this IList<double> values)
        {
            var maxValue = values[0];
            var i = 1;
            while (i < values.Count)
            {
                if (values[i] > maxValue)
                    maxValue = values[i];

                i++;
            }

            return maxValue;
        }

        public static double Max(this IList<float> values)
        {
            var maxValue = values[0];
            var i = 1;
            while (i < values.Count)
            {
                if (values[i] > maxValue)
                    maxValue = values[i];

                i++;
            }

            return maxValue;
        }

        public static T Min<T>(this IList<T> values, Func<T, T, bool> isASmallerThanB)
        {
            var minValue = values[0];
            var i = 1;

            while (i < values.Count)
            {
                if (isASmallerThanB(values[i], minValue))
                    minValue = values[i];

                i++;
            }

            return minValue;
        }

        #endregion
    }
}
