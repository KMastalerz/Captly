using captly.Extensions;
using captly.Interfaces;
using captly.Model;
using System.IO;
using System.Text.Json;

namespace captly.Services;
internal class ApplicationCacheService(IApplicationCacheStateService applicationCacheStateService) : IApplicationCacheService
{
    public async Task UpdateSubtitleCache(SubtitlesView subtitlesView)
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string cacheDirectory = Path.Combine(appDirectory, "Cache");

        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }

        var updatedFile = applicationCacheStateService.OpenedSubtitles.FirstOrDefault(f=>f.Path == subtitlesView.Path && f.Name == subtitlesView.Name);

        if(updatedFile is not null)
            updatedFile.Update(subtitlesView);

        await File.WriteAllTextAsync(Path.Combine(cacheDirectory, "subtitle-cache.json"), JsonSerializer.Serialize(applicationCacheStateService.OpenedSubtitles));
    }

    public async Task<List<Subtitles>> ReadCachedSubtitleState()
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string cacheDirectory = Path.Combine(appDirectory, "Cache");

        if (!Directory.Exists(cacheDirectory)) {
            return [];
        }

        string cacheFile = Path.Combine(cacheDirectory, "subtitle-cache.json");
        if (!File.Exists(cacheFile)) {
            return [];
        }

        string json = await File.ReadAllTextAsync(cacheFile);
        return JsonSerializer.Deserialize<List<Subtitles>>(json)!;
    }
}
