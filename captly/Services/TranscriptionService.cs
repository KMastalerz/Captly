using captly.Constants;
using captly.Extensions;
using captly.Interfaces;
using captly.Models;
using captly.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace captly.Services;

public class TranscriptionService(IAppConfigurationService appConfigurationService) : ITranscriptionService
{
    // Main method to transcribe a file using the provided TranscriptionViewModel and CancellationToken.
    public async Task TranscribeFile(TranscriptionViewModel transcriptionViewModel, CancellationToken cancellationToken)
    {
        // Get the application's base directory.
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        // Build the temporary directory path for transcriptions.
        string tempDirectory = Path.Combine(appDirectory, "Temp");
        tempDirectory = Path.Combine(appDirectory, "Transcriptions");

        // Create the temporary directory if it does not exist.
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }

        // Get the path to the Faster Whisper executable from the application configuration.
        var whisperPath = appConfigurationService.ApplicationConfiguration.FasterWhisperPath;

        // Start building the command prompt string with the whisper executable path.
        var prompt = $"\"{whisperPath}\" && ";

        // Get the executable name for Faster Whisper from the configuration.
        var whisperName = appConfigurationService.ApplicationConfiguration.FasterWhisperName;
        // Ensure the executable name ends with ".exe".
        if (!whisperName.EndsWith(".exe"))
            whisperName += ".exe";

        // Append the whisper executable name to the prompt.
        prompt += $"{whisperName}";

        // Build the full file name using the transcription view model data.
        var fileName = $"{transcriptionViewModel.Name}{transcriptionViewModel.Extension}";
        // Get the directory path where the file is located.
        var fileDirectory = transcriptionViewModel.Path;
        // Combine directory and file name to get the full file path.
        var filePath = Path.Combine(fileDirectory, fileName);
        // Append the file path (quoted) to the prompt.
        prompt += $" \"{filePath}\"";

        // Read any additional whisper options from the configuration.
        var whisperOptions = await appConfigurationService.ReadWhisperOptions();
        if (whisperOptions is not null)
        {
            // Append each checked option to the prompt.
            whisperOptions.Where(o => o.IsChecked).ToList().ForEach(o =>
            {
                if (o.Type == "bool")
                    prompt += $" {o.Original}";
                else
                    prompt += $" {o.Original} {o.Value}";
            });
        }

        // Append the output directory option to the prompt.
        prompt += $"  --output_dir \"{tempDirectory}\"";

        // Prepare the full command by switching to the correct drive/directory.
        var command = $"cd /d {prompt}";

        // Execute the command in a separate process.
        await RunProcessAsync(command, transcriptionViewModel, cancellationToken);

        // If the transcription process finished successfully, move the output file.
        if (transcriptionViewModel.Status == TranscriptionStatus.Finished)
        {
            // Build the subtitle file name (SRT file).
            var subtitleName = $"{transcriptionViewModel.Name}.srt";
            // Get the temporary path of the generated subtitle file.
            var tempPath = Path.Combine(tempDirectory, subtitleName);
            // Build the final output path where the file should be saved.
            var outputPath = Path.Combine(fileDirectory, subtitleName);

            // Check if the configuration requires archiving the old file.
            if (transcriptionViewModel.TranscriptionSetup.ArchiveOldFile)
            {
                // If an OldVersionPath is provided in the configuration.
                if (!string.IsNullOrEmpty(transcriptionViewModel.TranscriptionSetup.OldVersionPath))
                {
                    // Get the directory of the old version path.
                    var destinationDirectory = Path.GetDirectoryName(transcriptionViewModel.TranscriptionSetup.OldVersionPath)!;
                    var destinationPath = Path.Combine(destinationDirectory, subtitleName);

                    // If an existing output file exists, archive it.
                    if (File.Exists(outputPath))
                    {
                        // Create the destination directory if needed.
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }
                        // Move the existing file to the archive location, overwriting if necessary.
                        File.Move(outputPath, destinationPath, overwrite: true);
                    }
                }
                else
                {
                    // If no specific archive path is provided, use an "Archive" folder in the current file directory.
                    var destinationDirectory = Path.Combine(fileDirectory, "Archive")!;
                    var destinationPath = Path.Combine(destinationDirectory, subtitleName);

                    // Archive the file if it exists.
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

            // Check configuration to see if the file should be overwritten.
            if (transcriptionViewModel.TranscriptionSetup.OverriteFile)
            {
                // Overwrite the existing file with the new transcription.
                File.Move(tempPath, outputPath, overwrite: true);
            }
            // Otherwise, if a new version path is specified, move the file there.
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

    // Returns the global transcription setup from the application configuration.
    public TranscriptionSetup GetGlobalTranscriptionSetup()
    {
        return appConfigurationService.ApplicationConfiguration.TranscriptionSetup;
    }

    // Runs the given command in a new process asynchronously.
    private async Task RunProcessAsync(string command, TranscriptionViewModel transcriptionViewModel, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            // Configure the process start information.
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",                          // Use the Windows command interpreter.
                Arguments = $"/c {command}",                   // Pass the command to be executed.
                RedirectStandardOutput = true,                 // Redirect standard output to capture messages.
                RedirectStandardError = true,                  // Redirect error output to capture errors.
                UseShellExecute = false,                       // Required for redirection.
                CreateNoWindow = true,                         // Do not create a visible command window.
                StandardOutputEncoding = Encoding.UTF8,        // Set output encoding.
                StandardErrorEncoding = Encoding.UTF8          // Set error encoding.
            };

            // Create and start the process.
            using (Process process = new Process { StartInfo = psi })
            {
                // Handle output data received from the process.
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        // Update the UI on the application's dispatcher thread.
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // If the output contains a time span, calculate and display progress.
                            if (e.Data.HasTimeSpan())
                            {
                                var currentTimeSpan = e.Data.GetSubtitleTimeSpan(2);
                                if (currentTimeSpan is not null)
                                {
                                    double progress = (currentTimeSpan.Value.TotalSeconds / transcriptionViewModel.FileLength.TotalSeconds) * 100;
                                    transcriptionViewModel.CurrentMessage = $"Progress: {progress:F2}%";
                                }
                            }
                            // If the output indicates the operation is finished, update the status.
                            else if (e.Data.ToLower().Contains("operation finished"))
                            {
                                transcriptionViewModel.Status = TranscriptionStatus.Finished;
                                transcriptionViewModel.CurrentMessage = e.Data;
                            }
                            // Inform the user if the language detection process is ongoing.
                            else if (e.Data.ToLower().Contains("detecting language"))
                            {
                                transcriptionViewModel.CurrentMessage = "Detecting language...";
                            }
                            // Default message to the user.
                            else
                            {
                                transcriptionViewModel.CurrentMessage = "Please wait...";
                            }
                        });
                    }
                };

                // Handle error data received from the process.
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // Provide a custom message when downloading the model.
                            if (e.Data.ToLower().Contains("model.bin"))
                            {
                                transcriptionViewModel.CurrentMessage = $"Downloading model: {e.Data}";
                            }
                            else
                            {
                                transcriptionViewModel.CurrentMessage = e.Data;
                            }
                        });
                    }
                };

                // Start the process.
                process.Start();
                // Begin asynchronous read operations on the process's output and error streams.
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Monitor for cancellation requests while the process is running.
                while (!process.HasExited)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        App.Current.Dispatcher.InvokeAsync(() =>
                        {
                            transcriptionViewModel.CurrentMessage = $"Cancelled";
                        });
                        process.Kill(); // Kill the process if cancellation is requested.
                    }
                    // Sleep briefly before checking again.
                    Thread.Sleep(200);
                }
            }
        }, cancellationToken);
    }
}
