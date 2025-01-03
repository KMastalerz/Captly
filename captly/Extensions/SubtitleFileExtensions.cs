using captly.Model;

namespace captly.Extensions;
internal static class SubtitleFileExtensions
{
    public static Subtitles Update(this Subtitles subtitleFile, SubtitlesView extendedSubtitleFile)
    {
        subtitleFile.Name = extendedSubtitleFile.Name;
        subtitleFile.Path = extendedSubtitleFile.Path;
        subtitleFile.Extension = extendedSubtitleFile.Extension;
        subtitleFile.Status = extendedSubtitleFile.Status;
        subtitleFile.Encoding = extendedSubtitleFile.Encoding;
        subtitleFile.Progress = extendedSubtitleFile.Progress;
        subtitleFile.RequestCount = extendedSubtitleFile.RequestCount;
        subtitleFile.FailedRequestCount = extendedSubtitleFile.FailedRequestCount;
        subtitleFile.InputTokenCount = extendedSubtitleFile.InputTokenCount;
        subtitleFile.OutputTokenCount = extendedSubtitleFile.OutputTokenCount;
        subtitleFile.ElapsedTime = extendedSubtitleFile.ElapsedTime;
        subtitleFile.Error = extendedSubtitleFile.Error;
        subtitleFile.Language = extendedSubtitleFile.Language;
        subtitleFile.LastTranslatedID = extendedSubtitleFile.LastTranslatedID;
        return subtitleFile;
    } 
}
    