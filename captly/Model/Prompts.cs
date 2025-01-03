namespace captly.Model;
public class Prompts
{
    public string AdditionalFileOption { get; set; } = "Create additional file with language code for example \"_en\" or \"_pl\" at the end of name.";
    public string SubFolderOption { get; set; } = "Insert translated file into selected Language sub-folder.";
    public string OverriteFileOption { get; set; } = "Create temporarty file, but overrite original when translation is finished.";
    public string CustomSaveOption { get; set; } = "Save translated file in custom location and under custom name.";
    public string Name { get; set; } = "Name";
    public string Extension { get; set; } = "Extension";
    public string Encoding { get; set; } = "Encoding";
    public string Status { get; set; } = "Status";
    public string Settings { get; set; } = "Settings";
    public string Translate { get; set; } = "Translate";
    public string Pause { get; set; } = "Pause";
    public string Continue { get; set; } = "Continue";
    public string Retry { get; set; } = "Retry";
    public string Remove { get; set; } = "Remove";
    public string ElapsedTime { get; set; } = "Elapsed Time";
    public string RequestFailedRequest { get; set; } = "Request \\ Failed Request";
    public string InputOutputTokens { get; set; } = "Input \\ Output Tokens";
    public string SelectFolder { get; set; } = "Select folder";
    public string SelectFile { get; set; } = "Select file";
    public string TranslateAllFiles { get; set; } = "Translate all files";
    public string FilePath { get; set; } = "File path";
    public string FileName { get; set; } = "File name";
    public string Language { get; set; } = "Language";
}
