using System;
using System.Threading;

namespace TT.Services.Utilities
{
    public static class TimerExtensions
    {
        public static void Start(this Timer timer, int interval)
            => timer?.Change(TimeSpan.FromMilliseconds(interval), Timeout.InfiniteTimeSpan);

        public static void Stop(this Timer timer)
            => timer?.Change(Timeout.Infinite, 0);
    }
}
