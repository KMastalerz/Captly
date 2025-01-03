using System.Text.RegularExpressions;

namespace captly.Extensions;
internal static class StringExtensions
{
    public static string ExtrapolateJson(this string text)
    {
        // Define a regular expression pattern to match JSON objects
        string pattern = @"\[.*\]";

        // Use Regex.Match to find the first occurrence of a JSON object
        Match match = Regex.Match(text, pattern);

        if (match.Success)
        {
            // Extract the JSON string
            return match.Value;
        }
        else
        {
            return text;
        }
    }
}
