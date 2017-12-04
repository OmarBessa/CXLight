namespace CXLight.DataStructures.MultiMap.Exts
{
    using System.Collections.Generic;

    public static class MultiMapExt
    {
        public static List<T1> GetKeysWithValue<T1, T2>(this MultiMap<T1, T2> multimap, T2 value)
        {
            var result = new List<T1>();

            foreach (var itemAndCategory in multimap)
                if (itemAndCategory.Value.Equals(value)) result.Add(itemAndCategory.Key);

            return result;
        }
    }
}
