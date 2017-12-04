namespace CXLight.Exts
{
    using System;
    using System.Threading;

    public static class MresExt
    {
        public static bool TryWait(this ManualResetEventSlim mres, CancellationToken token)
        {
            try
            {
                mres.Wait(token);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}