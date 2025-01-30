using captly.Constants;
using captly.Core.AsyncCommand;
using captly.Services;
using captly.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace captly.Views.Transcription;

public class TrasncriptionListCommands
{
    public class TranscribeCommand(TranscriptionListViewModel transcriptionListViewModel) : AsyncCommand
    {

        public event EventHandler? CanExecuteChanged;

        public override bool CanExecute(object? parameter = null)
        {
            return !transcriptionListViewModel.TranscriptionFiles.Any(t => t.Status == TranscriptionStatus.Transcribing);
        }

        public override async Task ExecuteAsync(object? parameter = null)
        {
            var transcriptionService  = App.GetService<ITranscriptionService>();

            if (parameter is TranscriptionViewModel transcriptionViewModel)
            {
                transcriptionViewModel.CancellationTokenSource?.Cancel();
                transcriptionViewModel.CancellationTokenSource = new CancellationTokenSource();

                App.Current.Dispatcher.Invoke(() =>
                {
                    transcriptionViewModel.Status = TranscriptionStatus.Transcribing;
                });

                await transcriptionService.TranscribeFile(transcriptionViewModel, transcriptionViewModel.CancellationTokenSource.Token);
            }
        }
    }

    public class SelectFilesCommand() : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            var cacheService = App.GetService<ICacheService>();

            var transcriptionService = App.GetService<ITranscriptionService>();

            if (parameter is TranscriptionListViewModel transcriptionListViewModel)
            {
                var openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "Media Files (*.mkv;*.mp3;*.mp4)|*.mkv;*.mp3;*.mp4",
                    Title = "Select Media Files"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var filePath in openFileDialog.FileNames)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath); // Keeps the dot (e.g., .mp3)
                        var fileInfo = new FileInfo(filePath);

                        TimeSpan duration = TimeSpan.Zero;
                        using (var tagFile = TagLib.File.Create(filePath))
                        {
                            duration = tagFile.Properties.Duration;
                        }

                        var newTranscription = new TranscriptionViewModel
                        {
                            Name = fileName,
                            Path = Path.GetDirectoryName(filePath) ?? string.Empty,
                            Extension = extension,
                            FileSize = fileInfo.Length,
                            FileLength = duration,
                            Status = TranscriptionStatus.New,
                            TranscriptionSetup = transcriptionService.GetGlobalTranscriptionSetup()
                        };

                        if(transcriptionListViewModel.TranscriptionFiles is null)
                        {
                            transcriptionListViewModel.TranscriptionFiles = new();
                        }

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            transcriptionListViewModel.TranscriptionFiles.Add(newTranscription);
                        });
                    }
                }
                cacheService.CasheTranscriptions(transcriptionListViewModel.TranscriptionFiles);
            }
        }
    }

    public class StopCommand() : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            var cacheService = App.GetService<ICacheService>();
            var transcriptionListViewModel = App.GetService<TranscriptionListViewModel>();

            if (parameter is TranscriptionViewModel transcriptionViewModel)
            {
                transcriptionViewModel.CancellationTokenSource?.Cancel();
                App.Current.Dispatcher.InvokeAsync(() =>
                {
                    transcriptionViewModel.Status = TranscriptionStatus.Cancelled;
                });
                cacheService.CasheTranscriptions(transcriptionListViewModel.TranscriptionFiles);
            }  
        }
    }

    public class RemoveCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            var cacheService = App.GetService<ICacheService>();
            var transcriptionListViewModel = App.GetService<TranscriptionListViewModel>();

            if (parameter is TranscriptionViewModel transcriptionViewModel)
            {
                transcriptionListViewModel.TranscriptionFiles.Remove(transcriptionViewModel);
                cacheService.CasheTranscriptions(transcriptionListViewModel.TranscriptionFiles);
            }
        }
    }
}
