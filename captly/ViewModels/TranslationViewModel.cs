using captly.Constants;
using captly.Core;
using captly.Models;
using System.IO;
using System.Text;

namespace captly.ViewModels;

public class TranslationViewModel: BaseViewModel
{
    public TranslationViewModel(CacheTranslation cacheTranslation)
    {


        name = cacheTranslation.Name;
        path = cacheTranslation.Path;
        extension = cacheTranslation.Extension;
        fileSize = cacheTranslation.FileSize;
        status = cacheTranslation.Status;
        requestCount = cacheTranslation.RequestCount;
        failedRequestCount = cacheTranslation.FailedRequestCount;
        progress = cacheTranslation.Progress;
        translationSetup = cacheTranslation.TranslationSetup;

        using (var reader = new StreamReader($"{path}\\{name}{extension}", Encoding.Default, true))
        {
            encoding = reader.CurrentEncoding;
        }

        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string tempDirectory = System.IO.Path.Combine(appDirectory, "Temp");
        tempDirectory = System.IO.Path.Combine(appDirectory, "Translations");

        if (File.Exists(tempDirectory))
        {
            var json = File.ReadAllText(tempDirectory);
        }
    }

    public TranslationViewModel() { }

    private string name = default!;
    private string path = default!;
    private string extension = default!;
    private long fileSize = default!;
    private int requestCount = 0;
    private int failedRequestCount = 0;
    private int progress = 0;
    private TranslationStatus status = default!;
    private TranslationSetup translationSetup = default!;
    private Encoding encoding = Encoding.UTF8;
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
    public int RequestCount
    {
        get => requestCount;
        set => SetProperty(ref requestCount, value);
    }
    public int FailedRequestCount
    {
        get => failedRequestCount;
        set => SetProperty(ref failedRequestCount, value);
    }
    public int Progress
    {
        get => progress;
        set => SetProperty(ref progress, value);
    }
    public TranslationStatus Status
    {
        get => status;
        set => SetProperty(ref status, value);
    }
    public TranslationSetup TranslationSetup
    {
        get => translationSetup;
        set => SetProperty(ref translationSetup, value);
    }
    public List<Subtitle> Subtitles { get; set; } = [];
    public Encoding Encoding
    {
        get => encoding;
        set => SetProperty(ref encoding, value);
    }
    public CancellationTokenSource? CancellationTokenSource
    {
        get => cancellationTokenSource;
        set => SetProperty(ref cancellationTokenSource, value);
    }

    public void CancelTranslation()
    {
        CancellationTokenSource?.Cancel();
    }
}
