namespace CXLight.Kits
{
    using System;
    using Exts;

    public static class StringKit
    {
        private const string AlphanumericalDict = "abcdefjklmnopqrstuvwxyzABCDEFJKLMNOPQRSTUVWXYZ0123456789";

        public static string GetRandomAlphanumerical(int size)
        {
            return AlphanumericalDict.GetRandom(size);
        }

        public static string GetZeroPaddedNumber(int number, int digits, int numberBase = 10)
        {
            var result = "";
            var cursor = Math.Pow(numberBase, digits);

            var i = 0;
            while (cursor > 1)
            {
                if (cursor > number)
                {
                    i++;
                    cursor /= numberBase;
                }
                else if (cursor <= number)
                {
                    break;
                }
            }

            i--;

            var j = 0;
            while (j < i)
            {
                result += "0";
                j++;
            }

            return result + number;
        }
    }
}
