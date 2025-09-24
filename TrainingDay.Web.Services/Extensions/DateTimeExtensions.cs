namespace TrainingDay.Web.Services.Extensions;

public class DateTimeExtensions
{
    public static bool TryParseZone(string input, out TimeSpan result)
    {
        result = default;

        if (string.IsNullOrEmpty(input))
            return false;

        if (!(input.StartsWith("+") || input.StartsWith("-")))
            return false;

        // Strip the sign for parsing
        var withoutSign = input.Substring(1);

        // Validate exact hh:mm
        if (!TimeSpan.TryParseExact(withoutSign, @"hh\:mm", null, out result))
            return false;

        // Restore sign for TimeSpan
        if (input.StartsWith("-"))
            result = -result;

        return true;
    }
}