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
    public string CustomConfiguration {  get; set; } = default!;
    public bool MoveToTranslationAfterFinish { get; set; } = true;
}

public class TranslationSetup
{
    public bool OverriteFile { get; set; } = true;
    public bool ArchiveOldFile { get; set; } = false;
    public string OldVersionPath { get; set; } = default!;
    public string NewVersionPath { get; set; } = default!;
    public bool RunOllama {  get; set; } = false;
    public bool RunOpenAiApi { get; set; } = true;
    public string OpenAiApiKey { get; set; } = "";
    public string ModelName { get; set; } = "deepseek-r1:8b";
    public string DefaultLanguage { get; set; } = "Polish";
    public int SubtitleRequestCount { get; set; } = 15;
    public int RetryCount { get; set; } = 3;
}