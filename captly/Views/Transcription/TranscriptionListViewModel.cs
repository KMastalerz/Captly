using captly.Core;
using captly.Core.AsyncCommand;
using captly.Services;
using captly.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static captly.Views.Transcription.TrasncriptionListCommands;

namespace captly.Views.Transcription;

public class TranscriptionListViewModel: BaseViewModel
{
    public readonly ICacheService cacheService;
    public TranscriptionListViewModel(ICacheService cacheService)
    {
        TranscribeCommand = new TranscribeCommand(this);
        this.cacheService = cacheService;
        _ = Initialize();
    }

    private async Task Initialize()
    {
        var cache = await cacheService.ReadCacheDirectory();
        if (cache is not null)
        {
            var transcriptions = cache.Transcriptions.Select(n => new TranscriptionViewModel(n));
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                TranscriptionFiles = new(transcriptions ?? []);
            });
        }
    }

    private ObservableCollection<TranscriptionViewModel> transcriptionFiles = new();
    public ObservableCollection<TranscriptionViewModel> TranscriptionFiles
    {
        get => transcriptionFiles;
        set => SetProperty(ref transcriptionFiles, value);
    }

    public IAsyncCommand TranscribeCommand { get; }
    public ICommand SelectFilesCommand { get; } = new SelectFilesCommand();
    public ICommand StopCommand { get; } = new StopCommand();
    public ICommand RemoveCommand { get; } = new RemoveCommand();
}
