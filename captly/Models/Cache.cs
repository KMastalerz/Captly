using captly.Constants;

namespace captly.Models;

public class Cache
{
    public IEnumerable<CacheTranscription> Transcriptions { get; set; } = [];
    public IEnumerable<CacheTranslation> Translations { get; set; } = [];
}

public class CacheTranscription
{
    public string Name { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public TranscriptionStatus Status { get; set; } = default!;
    public TimeSpan FileLength { get; set; } = default!;
    public TranscriptionSetup TranscriptionSetup { get; set; } = default!;
}

public class CacheTranslation
{
    public string Name { get; set; } = default!;
    public string Path { get; set; } = default!;
    public string Extension { get; set; } = default!;
    public long FileSize { get; set; } = default!;
    public TranslationStatus Status { get; set; } = default!;
    public int RequestCount {  get; set; } = 0;
    public int FailedRequestCount {  get; set; } = 0;
    public int Progress {  get; set; } = 0;
    public TranslationSetup TranslationSetup { get; set; } = default!;
}