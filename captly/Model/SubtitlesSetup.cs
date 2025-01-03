
namespace captly.Model;

public class SubtitlesSetup
{
    public bool CreateAdditionalFile { get; set; } = false;
    public bool CreateLanguageSubFolder { get; set; } = true;
    public bool OverriteExistingFile { get; set; } = false;
    public bool SaveCustomFile { get; set; } = false;
    public string? CustomFileFullPath { get; set; } = null;
}
