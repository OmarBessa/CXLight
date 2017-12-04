namespace CXLight.Exts
{
    using System;
    using System.Collections.Generic;

    public static class DictionaryExt
    {
        /// <summary>
        /// Proud to have written the same code as Jon Skeet independently. I'm awesome. 8|
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> source,
            TKey key,
            TValue defaultValue
        )
        {
            return source.TryGetValue(key, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// Reverses the mapping.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            var result = new Dictionary<TValue, TKey>();

            foreach (var keyValuePair in source)
                result[keyValuePair.Value] = keyValuePair.Key;

            return result;
        }

        public static bool TryRemoveKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key) && dictionary.Remove(key);
        }

        /// <summary>
        /// Pure functional Map.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="source"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static Dictionary<T1, T3> Map<T1, T2, T3>(this Dictionary<T1, T2> source, Func<T2, T3> fn)
        {
            var result = new Dictionary<T1, T3>();
            foreach (var pair in source)
            {
                result[pair.Key] = fn(pair.Value);
            }

            return result;
        }

        /// <summary>
        /// Pure functional Map on two functions {key,value}.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyFn"></param>
        /// <param name="valueFn"></param>
        /// <returns></returns>
        public static Dictionary<TA, TB> Map<T1, T2, TA, TB>(this Dictionary<T1, T2> source, Func<T1, TA> keyFn, Func<T2, TB> valueFn)
        {
            var result = new Dictionary<TA, TB>();
            foreach (var pair in source)
            {
                result[keyFn(pair.Key)] = valueFn(pair.Value);
            }

            return result;
        }

        /// <summary>
        /// Pure functional Map on two functions {keyA,valueA} => {keyB,valueB}. ValueA is f(keyB, valueA).
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyFn"></param>
        /// <param name="valueFn"></param>
        /// <returns></returns>
        public static Dictionary<TA, TB> Map<T1, T2, TA, TB>(this Dictionary<T1, T2> source, Func<T1, TA> keyFn, Func<TA, T2, TB> valueFn)
        {
            var result = new Dictionary<TA, TB>();
            foreach (var pair in source)
            {
                var keyVal = keyFn(pair.Key);
                result[keyVal] = valueFn(keyVal, pair.Value);
            }

            return result;
        }
    }
}
