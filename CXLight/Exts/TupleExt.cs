namespace CXLight.Exts
{
    using System;
    public static class TupleExt
    {
        /// <summary>
        /// One to one mapping on a duple.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <param name="source"></param>
        /// <param name="fnA"></param>
        /// <param name="fnB"></param>
        /// <returns></returns>
        public static Tuple<TA, TB> Map<T1, T2, TA, TB>(this Tuple<T1, T2> source, Func<T1, TA> fnA, Func<T2, TB> fnB)
        {
            return Tuple.Create(fnA(source.Item1), fnB(source.Item2));
        }

        /// <summary>
        /// Maps a Tuple as if its elements were an array. 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TA"></typeparam>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static Tuple<TA, TA> Map<T1, TA>(this Tuple<T1, T1> source, Func<T1, TA> fn)
        {
            return Tuple.Create(fn(source.Item1), fn(source.Item2));
        }
    }
}
