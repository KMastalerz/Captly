using captly.Models;

namespace captly.Extensions;
public static class ListExtensions
{
    public static string ParseSubtitlesToSrt(this IEnumerable<Subtitle> list)
    {
        var result = "";
        foreach (var subtitle in list)
        {
            result += $"{subtitle.Index}.\n{subtitle.StartTime} --> {subtitle.EndTime}\n{subtitle.Text}\n";
        }
        return result;
    }
}
