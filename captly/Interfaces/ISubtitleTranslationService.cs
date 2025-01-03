using captly.Model;

namespace captly.Interfaces;
public interface ISubtitleTranslationService
{
    Task PauseTranslation();
    Task StartTranslation(SubtitlesView subtitlesView);
}