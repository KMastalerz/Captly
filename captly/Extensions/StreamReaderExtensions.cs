using captly.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace captly.Extensions;

public static class StreamReaderExtensions
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
            if (string.IsNullOrWhiteSpace(line)) 
            {
                if (currentSubtitle.Index > 0) 
                {
                    currentSubtitle.Text = textBuilder.ToString().Trim();
                    subtitles.Add(currentSubtitle);

                    currentSubtitle = new Subtitle();
                    textBuilder.Clear();
                }
            }
            else if (int.TryParse(line, out int index)) 
            {
                currentSubtitle.Index = index;
            }
            else if (Regex.IsMatch(line, @"\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}")) 
            {
                var times = line.Split(" --> ");
                currentSubtitle.StartTime = TimeSpan.ParseExact(times[0], @"hh\:mm\:ss\,fff", null);
                currentSubtitle.EndTime = TimeSpan.ParseExact(times[1], @"hh\:mm\:ss\,fff", null);
            }
            else 
            {
                textBuilder.AppendLine(line);
            }
        }

        if (currentSubtitle.Index > subtitles.Count) 
        {
            currentSubtitle.Text = textBuilder.ToString().Trim();
            subtitles.Add(currentSubtitle);

            currentSubtitle = new Subtitle();
            textBuilder.Clear();
        }

        return subtitles;
    }
}
