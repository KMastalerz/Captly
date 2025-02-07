using captly.Core.AsyncCommand;
using captly.Services;
using captly.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using captly.Core;
using static captly.Views.Translation.TranslationListCommands;

namespace captly.Views.Translation;

public class TranslationListViewModel: BaseViewModel
{
    public readonly ICacheService cacheService;

    public TranslationListViewModel(ICacheService cacheService)
    {
        TranslateCommand = new TranslateCommand(this);
        this.cacheService = cacheService;
        _ = Initialize();
    }

    private async Task Initialize()
    {
        var cache = await cacheService.ReadCacheDirectory();
        if (cache is not null)
        {
            var translations = cache.Translations.Select(n => new TranslationViewModel(n));
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                TranslationFiles = new(translations ?? []);
            });
        }
    }

    private ObservableCollection<TranslationViewModel> translationFiles = new();
    public ObservableCollection<TranslationViewModel> TranslationFiles
    {
        get => translationFiles;
        set => SetProperty(ref translationFiles, value);
    }

    public IAsyncCommand TranslateCommand { get; }
    public ICommand SelectFilesCommand { get; } = new SelectFilesCommand();
    public ICommand StopCommand { get; } = new StopCommand();
    public ICommand RemoveCommand { get; } = new RemoveCommand();
}
