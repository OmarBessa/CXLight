namespace CXLight.Kits
{
    using System;
    using System.Diagnostics;

    public static class BenchKit
    {
        private static long GetIterationTicks(int runs)
        {
            var sw = new Stopwatch();

            sw.Start();
            var i = 0;
            while (i < runs)
            {
                i++;
            }
            sw.Stop();

            return sw.ElapsedTicks;
        }

        public static double MeteredCall(Action action, int runs = 1)
        {
            var sw = new Stopwatch();

            if (runs == 1)
            {
                sw.Start();
                action();
                sw.Stop();

                return sw.ElapsedTicks;
            }

            sw.Start();

            var i = 0;
            while (i < runs)
            {
                action();
                i++;
            }

            sw.Stop();

            return (sw.ElapsedTicks - GetIterationTicks(runs)) / (double)runs;
        }
    }
}
