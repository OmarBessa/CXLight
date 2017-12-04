namespace CXLight.Kits
{
    using System.Diagnostics;
    
    public static class SysInfoKit
    {
        public static float GetAvailableMbytes()
        {
            return new PerformanceCounter("Memory", "Available MBytes").NextValue();
        }

        public static float GetCommitLimit()
        {
            return new PerformanceCounter("Memory", "Commit Limit").NextValue();
        }
    }
}