using TrainingDay.Web.Services.Extensions;

namespace TrainingDay.Web.Services.Tests.Extensions
{
    public class DateTimeExtensionsTests
    {
        [Theory]
        [InlineData("+00:00", 0, 0)]
        [InlineData("-00:00", 0, 0)]
        [InlineData("+03:00", 3, 0)]
        [InlineData("-12:30", -12, -30)]
        [InlineData("+23:59", 23, 59)]
        [InlineData("-23:59", -23, -59)]
        public void ValidOffsets_ShouldReturnTrue(string input, int expectedHours, int expectedMinutes)
        {
            var success = DateTimeExtensions.TryParseZone(input, out var result);

            Assert.True(success);
            Assert.Equal(expectedHours, result.Hours);
            Assert.Equal(expectedMinutes, result.Minutes);
        }

        [Theory]
        [InlineData("03:00")]     // Missing sign
        [InlineData("+-03:00")]   // Wrong sign
        [InlineData("+24:00")]    // Hour out of range
        [InlineData("-25:30")]    // Hour out of range
        [InlineData("+03:60")]    // Minute out of range
        [InlineData("+3:00")]     // Missing leading zero
        [InlineData("++03:00")]   // Double sign
        [InlineData("+0300")]     // Missing colon
        [InlineData("random")]    // Not a time
        [InlineData("")]
        [InlineData(null)]
        public void InvalidOffsets_ShouldReturnFalse(string input)
        {
            var success = DateTimeExtensions.TryParseZone(input, out var result);

            Assert.False(success);
        }
    }
}