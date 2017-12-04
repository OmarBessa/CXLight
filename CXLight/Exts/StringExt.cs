namespace CXLight.Exts
{
    using Kits;

    public static class StringExt
    {
        /// <summary>
        /// Creates a random string from the source data up until "size" chars.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetRandom(this string source, int size)
        {
            var result = "";

            var i = 0;
            while (i < size)
            {
                result += source[RngKit.Next(0, source.Length)];
                i++;
            }

            return result;
        }
    }
}
