using System.Text.RegularExpressions;

namespace captly.Extensions;

public static class StringExtensions
{
    private static readonly Regex TimeSpanRegex = new Regex(
        @"^\[\s*(?:(\d{1,2}):)?(\d{1,2}):(\d{2}\.\d{3})\s*-->\s*(?:(\d{1,2}):)?(\d{1,2}):(\d{2}\.\d{3})\s*\]",
        RegexOptions.Compiled);

    public static bool HasTimeSpan(this string value)
    {
        return TimeSpanRegex.IsMatch(value);
    }

    public static TimeSpan? GetSubtitleTimeSpan(this string value, int index = 1)
    {
        if (!value.HasTimeSpan()) return null;

        var match = TimeSpanRegex.Match(value);
        if (!match.Success) return null;

        // Determine which set of matches to use (1 = start time, 2 = end time)
        int offset = index == 2 ? 3 : 0;

        // Extract parts
        string hours = match.Groups[1 + offset].Value;    // Hours (if present)
        string minutes = match.Groups[2 + offset].Value;  // Minutes (always present)
        string seconds = match.Groups[3 + offset].Value;  // Seconds (always required)

        // Handle missing hours
        int h = string.IsNullOrEmpty(hours) ? 0 : int.Parse(hours);
        int m = int.Parse(minutes);
        double s = double.Parse(seconds, System.Globalization.CultureInfo.InvariantCulture);

        return new TimeSpan(h, m, 0) + TimeSpan.FromSeconds(s);
    }

    public static string ExtrapolateJson(this string text)
    {
        // Define a regular expression pattern to match JSON objects
        string pattern = @"\[[\s\S]*\]";

        // Use Regex.Match without needing to change regex options
        Match match = Regex.Match(text, pattern);

        if (match.Success)
        {
            return match.Value;
        }
        else
        {
            return text;
        }
    }
}
