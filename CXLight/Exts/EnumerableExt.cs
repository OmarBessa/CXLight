namespace CXLight.Exts
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExt
    {
        public static bool Some<T1>(this IEnumerable<T1> source, Func<T1, bool> predicate)
        {
            foreach (var item in source)
                if (predicate(item)) return true;

            return false;
        }

        /// <summary>
        /// In functional programming, filter is a higher-order function that processes a data structure (typically a list)
        /// in some order to produce a new data structure containing exactly those elements of the original data structure 
        /// for which a given predicate returns the boolean value true.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T1> Filter<T1>(this IEnumerable<T1> source, Func<T1, bool> predicate)
        {
            foreach (var item in source)
                if (predicate(item)) yield return item;
        }

        /// <summary>
        /// In functional programming, map is the name of a higher-order function that applies a given function to each element of a list, 
        /// returning a list of results in the same order.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static IEnumerable<T2> Map<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> map)
        {
            foreach (var item in source)
                yield return map(item);
        }

        public static void Walk<T1>(this IEnumerable<T1> source, Action<T1> fn)
        {
            foreach (var item in source)
                fn(item);
        }

        /// <summary>
        /// In functional programming, fold (also termed reduce, accumulate, aggregate, compress, or inject) refers to a family of higher-order functions
        ///  that analyze a recursive data structure and through use of a given combining operation, recombine the results of recursively processing 
        /// its constituent parts, building up a return value.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static T2 FoldL<T1, T2>(this IEnumerable<T1> source, T2 start, Func<T1, T2, T2> fn)
        {
            var cursor = start;
            foreach (var item in source)
                cursor = fn(item, cursor);

            return cursor;
        }
    }
}
