namespace captly.Models;

public class ApplicationConfiguration
{
    public string FasterWhisperPath { get; set; } = default!;
    public string FasterWhisperName { get; set; } = default!;
    public TranscriptionSetup TranscriptionSetup { get; set; } = new();
    public TranslationSetup TranslationSetup { get; set; } = new();
}

public class TranscriptionSetup
{
    public bool OverriteFile { get; set; } = true;
    public bool ArchiveOldFile { get; set; } = false;
    public string OldVersionPath { get; set; } = default!;
    public string NewVersionPath { get; set; } = default!;
    public string Model { get; set; } = "turbo";
    public string? SourceLanguage { get; set; } = null;
    public int MaxLineWidth { get; set; } = 42;
    public int MaxLineCount { get; set; } = 2;
    public int SubtitleRequestCount { get; set; } = 15;
    public int CachedMessageCount { get; set; } = 3;
    public string CustomConfiguration {  get; set; } = default!;
}

public class TranslationSetup
{
    public string OpenAiApiKey { get; set; } = default!;
    public string OpenAiApiModel { get; set; } = default!;
    public string DefaultLanguage { get; set; } = "Polish";
    public bool OverriteFile { get; set; } = true;
    public bool ArchiveOldFile { get; set; } = false;
    public string OldVersionPath { get; set; } = default!;
    public string NewVersionPath { get; set; } = default!;
}
