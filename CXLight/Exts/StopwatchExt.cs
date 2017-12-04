namespace CXLight.Exts
{
    using System.Diagnostics;

    public static class StopwatchExt
    {
        public static long ElapsedNs(this Stopwatch sw)
        {
            return sw.ElapsedTicks * 100;
        }
    }
}