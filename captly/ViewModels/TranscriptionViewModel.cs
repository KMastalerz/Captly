using captly.Constants;
using captly.Core;
using captly.Models;

namespace captly.ViewModels;

public class TranscriptionViewModel : BaseViewModel
{
    public TranscriptionViewModel(CacheTranscription cacheTranscription)
    {
        name = cacheTranscription.Name;
        path = cacheTranscription.Path;
        extension = cacheTranscription.Extension;
        fileSize = cacheTranscription.FileSize;
        fileLength = cacheTranscription.FileLength;
        status = cacheTranscription.Status;
        transcriptionSetup = cacheTranscription.TranscriptionSetup;
    }

    public TranscriptionViewModel() { }

    private string name = default!;
    private string path = default!;
    private string extension = default!;
    private long fileSize = default!;
    private TimeSpan fileLength = default!;
    private TranscriptionStatus status = default!;
    private string currentMessage = default!;
    private TranscriptionSetup transcriptionSetup = default!;
    private CancellationTokenSource? cancellationTokenSource;

    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }
    public string Path
    {
        get => path;
        set => SetProperty(ref path, value);
    }
    public string Extension
    {
        get => extension;
        set => SetProperty(ref extension, value);
    }
    public long FileSize
    {
        get => fileSize;
        set => SetProperty(ref fileSize, value);
    }
    public TimeSpan FileLength
    {
        get => fileLength;
        set => SetProperty(ref fileLength, value);
    }
    public TranscriptionStatus Status
    {
        get => status;
        set => SetProperty(ref status, value);
    }
    public string CurrentMessage
    {
        get => currentMessage;
        set => SetProperty(ref currentMessage, value);
    }
    public TranscriptionSetup TranscriptionSetup
    {
        get => transcriptionSetup;
        set => SetProperty(ref transcriptionSetup, value);
    }
    public CancellationTokenSource? CancellationTokenSource
    {
        get => cancellationTokenSource;
        set => SetProperty(ref cancellationTokenSource, value);
    }

    public void CancelTranscription()
    {
        CancellationTokenSource?.Cancel();
    }
}
