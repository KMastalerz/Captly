namespace captly.Model;

public class ApplicationConfiguration
{
    public string OpenAiApiKey { get; set; } = default!;
    public string OpenAiApiModel { get; set; } = default!;
    public SubtitlesSetup SubtitlesSetup { get; set; } = new();
}
