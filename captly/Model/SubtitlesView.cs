using captly.Core;
using captly.Enums;
using captly.Interfaces;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace captly.Model;
public class SubtitlesView: BaseViewModel
{
    public SubtitlesView(FileInfo fileInfo, Encoding encoding, IApplicationConfigurationService applicationConfigurationService)
    {
        var Language = applicationConfigurationService.Languages.FirstOrDefault(l =>
            l.EnglishName == applicationConfigurationService.ApplicationConfiguration.DefaultLanguage);

        subtitles.Path = fileInfo.FullName;
        subtitles.Name = System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName);
        subtitles.Extension = fileInfo.Extension;
        subtitles.Status = TranslationStatus.New;
        subtitles.Encoding = encoding;
        subtitles.Progress = 0.00;
        subtitles.RequestCount = 0;
        subtitles.FailedRequestCount = 0;
        subtitles.InputTokenCount = 0;
        subtitles.OutputTokenCount = 0;
        subtitles.ElapsedTime = string.Empty;
        subtitles.Error = string.Empty;
        subtitles.Language = Language?.NativeName ?? string.Empty;
        subtitles.LastTranslatedID = 0;
    }

    private Subtitles subtitles = new();
    private Stopwatch stopwatch = new();
    private CancellationTokenSource stopwatchCancellationTokenSource = new();
    private CancellationTokenSource translationCancellationTokenSource = new();

    public string Path
    {
        get => subtitles.Path;
        set => SetSubProperty(subtitles, s => s.Path, (s, v) => s.Path = v, value);
    }
    public string Name
    {
        get => subtitles.Name;
        set => SetSubProperty(subtitles, s => s.Name, (s, v) => s.Name = v, value);
    }
    public string Extension
    {
        get => subtitles.Extension;
        set => SetSubProperty(subtitles, s => s.Extension, (s, v) => s.Extension = v, value);
    }
    public TranslationStatus Status
    {
        get => subtitles.Status;
        set => SetSubProperty(subtitles, s => s.Status, (s, v) => s.Status = v, value);
    }
    public Encoding Encoding
    {
        get => subtitles.Encoding;
        set => SetSubProperty(subtitles, s => s.Encoding, (s, v) => s.Encoding = v, value);
    }
    public double Progress
    {
        get => subtitles.Progress;
        set => SetSubProperty(subtitles, s => s.Progress, (s, v) => s.Progress = v, value);
    }
    public int RequestCount
    {
        get => subtitles.RequestCount;
        set => SetSubProperty(subtitles, s => s.RequestCount, (s, v) => s.RequestCount = v, value);
    }
    public int FailedRequestCount
    {
        get => subtitles.FailedRequestCount;
        set => SetSubProperty(subtitles, s => s.FailedRequestCount, (s, v) => s.FailedRequestCount = v, value);
    }
    public int InputTokenCount
    {
        get => subtitles.InputTokenCount;
        set => SetSubProperty(subtitles, s => s.InputTokenCount, (s, v) => s.InputTokenCount = v, value);
    }
    public int OutputTokenCount
    {
        get => subtitles.OutputTokenCount;
        set => SetSubProperty(subtitles, s => s.OutputTokenCount, (s, v) => s.OutputTokenCount = v, value);
    }
    public string ElapsedTime
    {
        get => subtitles.ElapsedTime;
        set => SetSubProperty(subtitles, s => s.ElapsedTime, (s, v) => s.ElapsedTime = v, value);
    }
    public string Error
    {
        get => subtitles.Error;
        set => SetSubProperty(subtitles, s => s.Error, (s, v) => s.Error = v, value);
    }
    public string Language
    {
        get => subtitles.Language;
        set => SetSubProperty(subtitles, s => s.Language, (s, v) => s.Language = v, value);
    }
    public int LastTranslatedID
    {
        get => subtitles.LastTranslatedID;
        set => SetSubProperty(subtitles, s => s.LastTranslatedID, (s, v) => s.LastTranslatedID = v, value);
    }
    public SubtitlesSetup? SubtitlesSetup
    {
        get => subtitles.SubtitlesSetup;
        set => SetSubProperty(subtitles, s => s.SubtitlesSetup, (s, v) => s.SubtitlesSetup = v, value);
    }
    public CancellationToken TranslationToken => translationCancellationTokenSource.Token;

    public async Task StartElapsedTimeTracking()
    {
        // Parse the initial time from ElapsedTime, if available
        TimeSpan initialTime = TimeSpan.Zero;

        if (TimeSpan.TryParseExact(ElapsedTime, @"hh\:mm\:ss", null, out TimeSpan parsedTime))
        {
            initialTime = parsedTime;
        }

        // Start the stopwatch
        stopwatch.Start();

        // Start the stopwatch loop
        while (!stopwatchCancellationTokenSource.Token.IsCancellationRequested)
        {
            // Combine initial time with the stopwatch's elapsed time
            TimeSpan currentElapsedTime = initialTime + stopwatch.Elapsed;
            ElapsedTime = currentElapsedTime.ToString(@"hh\:mm\:ss");

            await Task.Delay(1000); // Update every second
        }
    }

    public void StopElapsedTimeTracking()
    {
        stopwatch.Stop();
        CancelElapsedTimeTracking();
    }

    public void CancelElapsedTimeTracking()
    {
        stopwatchCancellationTokenSource.Cancel();
    }

    public void PauseTranslation()
    {
        translationCancellationTokenSource.Cancel();
    }

    public void RestartTranslationToken()
    {
        translationCancellationTokenSource = new();
    }

    public async Task UpdateRequestCount()
    {
        await Application.Current.Dispatcher.BeginInvoke(() => {
            RequestCount++;
        });
    }

    public async Task UpdateProgress(double progress)
    {
        await Application.Current.Dispatcher.BeginInvoke(() => {
            Progress = progress;
        });
    }

    public async Task UpdateStatus(TranslationStatus status)
    {
        await Application.Current.Dispatcher.BeginInvoke(() => {
            Status = status;
        });
    }

    public async Task UpdateFailedRequestCount()
    {
        await Application.Current.Dispatcher.BeginInvoke(() =>
        {
            FailedRequestCount++;
        });
    }

    public async Task UpdateInputTokenCount(int count)
    {
        await Application.Current.Dispatcher.BeginInvoke(() =>
        {
            InputTokenCount = count;
        });
    }

    public async Task UpdateOutputTokenCount(int count)
    {
        await Application.Current.Dispatcher.BeginInvoke(() =>
        {
            OutputTokenCount = count;
        });
    }

    public async Task UpdateError(string error)
    {
        await Application.Current.Dispatcher.BeginInvoke(() =>
        {
            Error = error;
        });
    }
}
