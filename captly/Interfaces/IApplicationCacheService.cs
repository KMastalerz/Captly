using captly.Model;

namespace captly.Interfaces;
internal interface IApplicationCacheService
{
    Task UpdateSubtitleCache(SubtitlesView subtitlesView);
    Task<List<Subtitles>> ReadCachedSubtitleState();
}