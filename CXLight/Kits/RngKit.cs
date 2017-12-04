namespace CXLight.Kits
{
    using System;

    /// <summary>
    /// A single stop for all the libraries' random needs.
    /// </summary>
    public static class RngKit
    {
        private static readonly Random Rng = new Random();

        public static int Next(int inclusiveStart, int exclusiveEnd)
        {
            return Rng.Next(inclusiveStart, exclusiveEnd);
        }

        public static int Next(int exclusiveEnd)
        {
            return Next(0, exclusiveEnd);
        }

        public static void NextBytes(byte[] bytes)
        {
            Rng.NextBytes(bytes);
        }
    }
}
