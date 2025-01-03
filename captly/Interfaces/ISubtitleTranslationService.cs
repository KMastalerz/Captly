using captly.Model;

namespace captly.Interfaces;
internal interface ISubtitleTranslationService
{
    Task PauseTranslation();
    Task StartTranslation(SubtitlesView subtitlesView);
}