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
        status = cacheTranslation.Status;
        requestCount = cacheTranslation.RequestCount;
        failedRequestCount = cacheTranslation.FailedRequestCount;
        inputTokenCount = cacheTranslation.InputTokenCount;
        outputTokenCount = cacheTranslation.OutputTokenCount;
        progress = cacheTranslation.Progress;
        translationSetup = cacheTranslation.TranslationSetup;
        encoding = cacheTranslation.Encoding;

        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string tempDirectory = System.IO.Path.Combine(appDirectory, "Temp");

        if (File.Exists(tempDirectory))
        {
            var json = File.ReadAllText(tempDirectory);
        }
    }

    public TranslationViewModel() { }

    private string name = default!;
    private string path = default!;
    private TranslationStatus status = default!;
    private int requestCount = 0;
    private int failedRequestCount = 0;
    private int inputTokenCount = 0;
    private int outputTokenCount = 0;
    private int progress = 0;
    private TranslationSetup translationSetup = default!;
    private Encoding encoding = Encoding.UTF8;

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
    public TranslationStatus Status
    {
        get => status;
        set => SetProperty(ref status, value);
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
    public int InputTokenCount
    {
        get => inputTokenCount;
        set => SetProperty(ref inputTokenCount, value);
    }
    public int OutputTokenCount
    {
        get => outputTokenCount;
        set => SetProperty(ref outputTokenCount, value);
    }
    public int Progress
    {
        get => progress;
        set => SetProperty(ref progress, value);
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
}
