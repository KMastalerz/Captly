using captly.Constants;
using captly.Extensions;
using captly.Models;
using captly.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace captly.Services;

public class TranscriptionService : ITranscriptionService
{
    private readonly ApplicationConfiguration ApplicationConfiguration;
    public TranscriptionService(IConfiguration configuration)
    {
        ApplicationConfiguration = configuration.GetSection("ApplicationConfiguration").Get<ApplicationConfiguration>() ?? new();
    }
    public async Task TranscribeFile(TranscriptionViewModel transcriptionViewModel, CancellationToken cancellationToken)
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string tempDirectory = Path.Combine(appDirectory, "Temp");
        
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        var whisperPath = ApplicationConfiguration.FasterWhisperPath;

        var prompt = $"\"{whisperPath}\" && ";

        var whisperName = ApplicationConfiguration.FasterWhisperName;
        if (!whisperName.EndsWith(".exe"))
            whisperName += ".exe";

        prompt += $"{whisperName}";

        var fileName = $"{transcriptionViewModel.Name}{transcriptionViewModel.Extension}";
        var fileDirectory = transcriptionViewModel.Path;
        var filePath = Path.Combine(fileDirectory, fileName);
        prompt += $" \"{filePath}\"";
        prompt += $" -m {transcriptionViewModel.TranscriptionSetup.Model}";
        prompt += $" --sentence";
        prompt += $" --max_line_width {transcriptionViewModel.TranscriptionSetup.MaxLineWidth}";
        prompt += $" --max_line_count {transcriptionViewModel.TranscriptionSetup.MaxLineCount}";
        prompt += $"  --output_dir \"{tempDirectory}\"";

        var command = $"cd /d {prompt}";

        await RunProcessAsync(command, transcriptionViewModel, cancellationToken);

        if(transcriptionViewModel.Status == TranscriptionStatus.Finished)
        {
            var subtitleName = $"{transcriptionViewModel.Name}.srt";
            var tempPath = Path.Combine(tempDirectory, subtitleName);
            var outputPath = Path.Combine(fileDirectory, subtitleName);


            if (transcriptionViewModel.TranscriptionSetup.ArchiveOldFile)
            {
                if (!string.IsNullOrEmpty(transcriptionViewModel.TranscriptionSetup.OldVersionPath))
                {
                    var destinationDirectory = Path.GetDirectoryName(transcriptionViewModel.TranscriptionSetup.OldVersionPath)!;
                    var destinationPath = Path.Combine(destinationDirectory, subtitleName);

                    if(File.Exists(outputPath))
                    {

                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        File.Move(outputPath, destinationPath, overwrite: true);
                    }
                }
                else
                {
                    var destinationDirectory = Path.Combine(fileDirectory, "Archive")!;
                    var destinationPath = Path.Combine(destinationDirectory, subtitleName);



                    if (File.Exists(outputPath))
                    {

                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        File.Move(outputPath, destinationPath, overwrite: true);
                    }
                }
            }

            if (transcriptionViewModel.TranscriptionSetup.OverriteFile)
            {
                File.Move(tempPath, outputPath, overwrite: true);
            }
            else if (!string.IsNullOrEmpty(transcriptionViewModel.TranscriptionSetup.NewVersionPath))
            {
                var destinationDirectory = Path.GetDirectoryName(transcriptionViewModel.TranscriptionSetup.NewVersionPath)!;
                var destinationPath = Path.Combine(destinationDirectory, fileName);

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                File.Move(tempPath, destinationPath, overwrite: true);
            }
        }
    }
    public TranscriptionSetup GetGlobalTranscriptionSetup()
    {
        return ApplicationConfiguration.TranscriptionSetup;
    }
    private async Task RunProcessAsync(string command, TranscriptionViewModel transcriptionViewModel, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.OutputDataReceived += (sender, e) =>
                {

                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if(e.Data.HasTimeSpan())
                            {
                                var currentTimeSpan = e.Data.GetSubtitleTimeSpan(2);
                                if (currentTimeSpan is not null)
                                {
                                    double progress = (currentTimeSpan.Value.TotalSeconds / transcriptionViewModel.FileLength.TotalSeconds) * 100;
                                    transcriptionViewModel.CurrentMessage = $"Progress: {progress:F2}%";
                                }
                            }
                            else if (e.Data.ToLower().Contains("operation finished"))
                            {
                                transcriptionViewModel.Status = TranscriptionStatus.Finished;
                                transcriptionViewModel.CurrentMessage = e.Data;
                            }
                            else if (e.Data.ToLower().Contains("detecting language"))
                            {
                                transcriptionViewModel.CurrentMessage = "Detecting language...";
                            }
                            else
                            {
                                transcriptionViewModel.CurrentMessage = "Please wait...";
                            }
                        });
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            if(e.Data.ToLower().Contains("model.bin"))
                            {
                                transcriptionViewModel.CurrentMessage = $"Downloading model: {e.Data}";
                            }
                        });
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Monitor cancellation
                while (!process.HasExited)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            transcriptionViewModel.CurrentMessage = $"Cancelled";
                        });
                        process.Kill();
                    }
                    Thread.Sleep(200);
                }
            }
        }, cancellationToken);
    }
}
