using captly.Core;
using captly.Interfaces;
using captly.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static captly.Views.TranslateCommands;

namespace captly.Views;
public class TranslateViewModel: BaseViewModel
{

    private readonly IPromptService promptService;
    public TranslateViewModel(
        ISubtitleTranslationService subtitleTranslateService, 
        IPromptService promptService, 
        IDisplayService displayService,
        ITranslateStateService translateStateService,
        IApplicationConfigurationService applicationConfigurationService)
    {
        SelectFolderCommand = new SelectFolder(applicationConfigurationService);
        DeleteFileCommand = new DeleteFile(this);
        SelectFileCommand = new SelectFile(applicationConfigurationService);
        OpenSettingsCommand = new OpenSettings(displayService, translateStateService);
        TranslateFileCommand = new TranslateFile(subtitleTranslateService);
        TranslateAllFilesCommand = new TranslateAllFiles(subtitleTranslateService);
        PauseTranslationCommand = new PauseTranslation(subtitleTranslateService);
        this.promptService = promptService;
    }


    //To be moved to separate window
    private ObservableCollection<SubtitlesView> files = [];
    public ObservableCollection<SubtitlesView> Files
    {
        get => files;
        set => SetProperty(ref files, value);
    }

    public Prompts Prompts => promptService.Prompts;

    public ICommand SelectFolderCommand { get; }
    public ICommand DeleteFileCommand { get; }
    public ICommand SelectFileCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public IAsyncCommand TranslateFileCommand { get; }
    public IAsyncCommand TranslateAllFilesCommand { get; }
    public IAsyncCommand PauseTranslationCommand { get; }
}
