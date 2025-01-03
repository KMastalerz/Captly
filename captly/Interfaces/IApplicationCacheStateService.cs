using captly.Model;

namespace captly.Interfaces;
public interface IApplicationCacheStateService
{
    List<Subtitles> OpenedSubtitles { get; set; }
}