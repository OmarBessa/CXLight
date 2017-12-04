namespace CXLight.Exts
{
    public static class DoubleExt
    {
        public static int FloorToInt(this double source)
        {
            return (int)System.Math.Floor(source);
        }
    }
}
