using captly.Enums;
using System.Text;

namespace captly.Model;

public class Subtitles
{
    public string Name { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string FullPath { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public TranslationStatus Status { get; set; } = TranslationStatus.New;
    public Encoding Encoding { get; set; } = default!;
    public int RequestCount { get; set; } = default!;
    public int FailedRequestCount { get; set; } = default!;
    public int InputTokenCount { get; set; } = default!;
    public int OutputTokenCount { get; set; } = default!;
    public string Language { get; set; } = default!;
    public int LastTranslatedID { get; set; } = 0;
    public double Progress { get; set; } = default!;
    public string ElapsedTime { get; set; } = default!;
    public string Error { get; set; } = default!;
    public SubtitlesSetup? SubtitlesSetup { get; set; } = null;
}
