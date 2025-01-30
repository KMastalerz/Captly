using captly.Models;
using captly.ViewModels;
using System.IO;
using System.Text.Json;

namespace captly.Services;

public class CacheService : ICacheService
{
    public async Task<Cache?> ReadCacheDirectory()
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string cacheDirectory = Path.Combine(appDirectory, "Cache");
        string cacheFile = Path.Combine(cacheDirectory, "Cache.json");

        if (!File.Exists(cacheFile))
            return null;

        string cacheJson = await File.ReadAllTextAsync(cacheFile);

        if (string.IsNullOrEmpty(cacheJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Cache>(cacheJson);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveCache(Cache cache)
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string cacheDirectory = Path.Combine(appDirectory, "Cache");
        string cacheFile = Path.Combine(cacheDirectory, "Cache.json");

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        if(!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }

        await File.WriteAllTextAsync(cacheFile, JsonSerializer.Serialize(cache, jsonOptions));
    }

    public async Task CasheTranslations(IEnumerable<TranslationViewModel> translations)
    {
        var cache = await ReadCacheDirectory() ?? new Cache();

        cache.Translations = translations.Select(t => new CacheTranslation()
        {
            Name = t.Name,
            Path = t.Path,
            Status = t.Status,
            RequestCount = t.RequestCount,
            FailedRequestCount = t.FailedRequestCount,
            InputTokenCount = t.InputTokenCount,
            OutputTokenCount = t.OutputTokenCount,
            Progress = t.Progress,
            TranslationSetup = t.TranslationSetup,
        });

        await SaveCache(cache);
    }

    public async Task CasheTranscriptions(IEnumerable<TranscriptionViewModel> transcriptions)
    {
        var cache = await ReadCacheDirectory() ?? new Cache();

        cache.Transcriptions = transcriptions.Select(t => new CacheTranscription()
        {
            Name = t.Name,
            Path = t.Path,
            Status = t.Status,
            Extension = t.Extension,
            FileSize = t.FileSize,
            FileLength = t.FileLength,
            TranscriptionSetup = t.TranscriptionSetup
        });

        await SaveCache(cache);
    }
}
