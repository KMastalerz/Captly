using captly.Models;
using captly.ViewModels;

namespace captly.Services;
public interface ICacheService
{
    Task CasheTranscriptions(IEnumerable<TranscriptionViewModel> transcriptions);
    Task CasheTranslations(IEnumerable<TranslationViewModel> translations);
    Task<Cache?> ReadCacheDirectory();
    Task SaveCache(Cache cache);
}