using captly.Core;
using captly.Interfaces;
using captly.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static captly.Views.TranslateCommands;

namespace captly.Views;
internal class TranslateViewModel: BaseViewModel
{

    private readonly IPromptService promptService;
    public TranslateViewModel(ISubtitleTranslationService subtitleTranslateService, IPromptService promptService)
    {
        SelectFolderCommand = new SelectFolder();
        DeleteFileCommand = new DeleteFile(this);
        SelectFileCommand = new SelectFile();
        TranslateFileCommand = new TranslateFile(subtitleTranslateService);
        TranslateAllFilesCommand = new TranslateAllFiles(subtitleTranslateService);
        PauseTranslationCommand = new PauseTranslation(subtitleTranslateService);
        Languages = Task.Run(()=> subtitleTranslateService.GetLanguages()).Result;
        this.promptService = promptService;
    }


    //To be moved to separate window
    private ObservableCollection<SubtitlesView> files = [];
    public ObservableCollection<SubtitlesView> Files
    {
        get => files;
        set => SetProperty(ref files, value);
    }

    private List<Language> languages = [];
    public List<Language> Languages
    {
        get => languages;
        set => SetProperty(ref languages, value);
    }

    public Prompts Prompts => promptService.Prompts;

    public ICommand SelectFolderCommand { get; }
    public ICommand DeleteFileCommand { get; }
    public ICommand SelectFileCommand { get; }
    public IAsyncCommand TranslateFileCommand { get; }
    public IAsyncCommand TranslateAllFilesCommand { get; }
    public IAsyncCommand PauseTranslationCommand { get; }
}
