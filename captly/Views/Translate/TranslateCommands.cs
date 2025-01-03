using captly.Core;
using captly.Enums;
using captly.Model;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace captly.Views;
internal class TranslateCommands
{
    public class SelectFolder() : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is TranslateViewModel translateViewModel)
            {
                var dialog = new OpenFolderDialog();
                if (dialog.ShowDialog() == true)
                {
                    translateViewModel.Files.Clear();
                    foreach (var file in Directory.GetFiles(dialog.FolderName, "*.srt", SearchOption.TopDirectoryOnly))
                    {
                        // Get file name, extension, and size 
                        using (var reader = new StreamReader(file, Encoding.Default, true))
                        {
                            reader.Peek(); // Force StreamReader to detect encoding
                            translateViewModel.Files.Add(new(new(file), reader.CurrentEncoding));
                        }
                    }
                }
            }
        }
    }

    public class SelectFile : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is TranslateViewModel translateViewModel)
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Select an SRT File",
                    Filter = "SRT Files (*.srt)|*.srt|All Files (*.*)|*.*",
                    Multiselect = false // Allow only single file selection
                };

                if (dialog.ShowDialog() == true)
                {
                    string selectedFile = dialog.FileName;

                    // Detect encoding and add file information
                    using (var reader = new StreamReader(selectedFile, Encoding.Default, true))
                    {
                        reader.Peek(); // Force StreamReader to detect encoding
                        translateViewModel.Files.Clear(); // Optionally clear existing files
                        translateViewModel.Files.Add(new(new(selectedFile), reader.CurrentEncoding));
                    }
                }
            }
        }
    }

    public class DeleteFile(TranslateViewModel translateViewModel) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is SubtitlesView subtitlesView)
            {
                translateViewModel.Files.Remove(subtitlesView);
            }
        }
    }

    public class TranslateFile(ISubtitleTranslationService subtitleTranslationService) : AsyncCommand
    {
        public event EventHandler? CanExecuteChanged;

        public override bool CanExecute(object? parameter = null)
        {
            if (parameter is SubtitlesView subtitlesView)
            {
                return subtitlesView.Status != TranslationStatus.Translating;
            }
            else return false;
        }

        public override async Task ExecuteAsync(object? parameter = null)
        {

            if (parameter is SubtitlesView subtitlesView)
            {
                try
                {
                    await subtitleTranslationService.StartTranslation(subtitlesView);
                }
                catch (Exception e)
                {
                    subtitlesView.StopElapsedTimeTracking();
                    await subtitlesView.UpdateError(e.Message);
                    await subtitlesView.UpdateStatus(TranslationStatus.Error);
                }
            }
        }
    }

    public class TranslateAllFiles(ISubtitleTranslationService subtitleTranslationService) : AsyncCommand
    {
        public event EventHandler? CanExecuteChanged;

        public override bool CanExecute(object? parameter = null)
        {
            if (parameter is TranslateViewModel translateViewModel)
            {
                return translateViewModel.Files.Count(x => x.Status == TranslationStatus.Translating) < 5;
            }
            else return false;
        }

        public override async Task ExecuteAsync(object? parameter = null)
        {
            if (parameter is TranslateViewModel translateViewModel)
            {
                var semaphore = new SemaphoreSlim(5 - translateViewModel.Files.Count(x => x.Status == TranslationStatus.Translating));
                var tasks = new List<Task>();


                foreach (var subtitlesView in translateViewModel.Files)
                {
                    // Do not translate if operation in progress
                    if (subtitlesView.Status == TranslationStatus.Translating) continue;
                    // Wait for an available slot
                    await semaphore.WaitAsync();

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await subtitleTranslationService.StartTranslation(subtitlesView);
                        }
                        catch(Exception e)
                        {
                            subtitlesView.StopElapsedTimeTracking();
                            await subtitlesView.UpdateError(e.Message);
                            await subtitlesView.UpdateStatus(TranslationStatus.Error);
                        }
                        finally
                        {
                            // Release the semaphore slot
                            semaphore.Release();
                        }
                    });

                    tasks.Add(task);
                }

                // Wait for all tasks to complete
                await Task.WhenAll(tasks);
            }
        }
    }

    public class PauseTranslation(ISubtitleTranslationService subtitleTranslationService) : AsyncCommand
    {
        public override bool CanExecute(object? parameter = null)
        {
            if (parameter is SubtitlesView subtitlesView)
            {
                return subtitlesView.Status == TranslationStatus.Translating;
            }
            else return false;
        }

        public override Task ExecuteAsync(object? parameter = null)
        {
            if (parameter is SubtitlesView subtitlesView)
            {
                await subtitleTranslationService.PauseTranslation(subtitlesView);
            }
        }
    }
}
