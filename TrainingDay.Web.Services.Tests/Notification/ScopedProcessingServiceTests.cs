using TrainingDay.Web.Services.Notification;

namespace TrainingDay.Web.Services.Tests.Notification
{
    public class ScopedProcessingServiceTests
    {
        private static readonly TimeSpan EightAm = new(8, 0, 0);

        [Theory]
        [InlineData("2026-10-17 04:59:30", "2026-10-17 05:00:30", 3, true)]   // 08:00 at +03:00 is inside the window
        [InlineData("2026-10-17 05:00:00", "2026-10-17 05:01:00", 3, false)]  // start is exclusive: already sent in the previous run
        [InlineData("2026-10-17 04:59:00", "2026-10-17 05:00:00", 3, true)]   // end is inclusive
        [InlineData("2026-10-17 07:59:00", "2026-10-17 08:00:00", 0, true)]   // UTC zone
        [InlineData("2026-10-17 12:00:00", "2026-10-17 12:01:00", 3, false)]  // nowhere near 08:00 local
        [InlineData("2026-10-17 12:59:30", "2026-10-17 13:00:30", -5, true)]  // negative offset
        [InlineData("2026-10-16 22:59:30", "2026-10-16 23:00:30", 9, true)]   // local day is ahead of the UTC day
        public void IsTimeReached_DetectsSendTimeInWindow(string fromUtc, string toUtc, int zoneHours, bool expected)
        {
            var result = ScopedProcessingService.IsTimeReached(
                DateTime.Parse(fromUtc), DateTime.Parse(toUtc), TimeSpan.FromHours(zoneHours), EightAm);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("+03:00", 3, 0)]
        [InlineData("-05:30", -5, -30)]
        [InlineData("+00:00", 0, 0)]
        [InlineData("03:00", 3, 0)]     // unsigned value from older app versions
        [InlineData("garbage", 3, 0)]   // falls back to the default zone
        [InlineData("", 3, 0)]
        public void ParseOrDefault_ParsesSignedZones(string zone, int expectedHours, int expectedMinutes)
        {
            var result = ScopedProcessingService.ParseOrDefault(zone);

            Assert.Equal(new TimeSpan(expectedHours, expectedMinutes, 0), result);
        }

        [Fact]
        public void IsTimeReached_ConsecutiveWindows_FireExactlyOnce()
        {
            var zone = TimeSpan.FromHours(3);
            var start = new DateTime(2026, 10, 17, 0, 0, 0);
            var fired = 0;

            // A full day of one-minute runs, each starting where the previous one ended.
            for (var i = 0; i < 24 * 60; i++)
            {
                var from = start.AddMinutes(i);
                if (ScopedProcessingService.IsTimeReached(from, from.AddMinutes(1), zone, EightAm))
                {
                    fired++;
                }
            }

            Assert.Equal(1, fired);
        }
    }
}
