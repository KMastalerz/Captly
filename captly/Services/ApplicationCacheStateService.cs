using captly.Interfaces;
using captly.Model;

namespace captly.Services;
internal class ApplicationCacheStateService : IApplicationCacheStateService
{
    public List<Subtitles> OpenedSubtitles { get; set; } = [];
}
