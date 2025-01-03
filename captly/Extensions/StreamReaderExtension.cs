using captly.Model;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace captly.Extensions;
internal static class StreamReaderExtension
{
    public static List<Subtitle> SrtToList(this StreamReader streamReader)
    {
        var subtitles = new List<Subtitle>();
        var currentSubtitle = new Subtitle();
        var textBuilder = new StringBuilder();

        string? line;
        while ((line = streamReader.ReadLine()) != null)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line)) // End of current subtitle
            {
                if (currentSubtitle.Index > 0) // Add subtitle to the list
                {
                    currentSubtitle.Text = textBuilder.ToString().Trim();
                    subtitles.Add(currentSubtitle);

                    currentSubtitle = new Subtitle();
                    textBuilder.Clear();
                }
            }
            else if (int.TryParse(line, out int index)) // Index line
            {
                currentSubtitle.Index = index;
            }
            else if (Regex.IsMatch(line, @"\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}")) // Time line
            {
                var times = line.Split(" --> ");
                currentSubtitle.StartTime = TimeSpan.ParseExact(times[0], @"hh\:mm\:ss\,fff", null);
                currentSubtitle.EndTime = TimeSpan.ParseExact(times[1], @"hh\:mm\:ss\,fff", null);
            }
            else // Text line
            {
                textBuilder.AppendLine(line);
            }
        }

        // Make sure to add last line!
        if (currentSubtitle.Index > subtitles.Count) // Add subtitle to the list
        {
            currentSubtitle.Text = textBuilder.ToString().Trim();
            subtitles.Add(currentSubtitle);

            currentSubtitle = new Subtitle();
            textBuilder.Clear();
        }

        return subtitles;
    }
}
