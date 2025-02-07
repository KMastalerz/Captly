using captly.Constants;
using captly.Core.AsyncCommand;
using captly.Interfaces;
using captly.Services;
using captly.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace captly.Views.Translation;
public class TranslationListCommands
{
    public class TranslateCommand(TranslationListViewModel translationListViewModel) : AsyncCommand
    {
        public override bool CanExecute(object? parameter = null)
        {
            return !translationListViewModel.TranslationFiles.Any(t => t.Status == TranslationStatus.Translating);
        }

        public override async Task ExecuteAsync(object? parameter = null)
        {
            var translationService = App.GetService<ITranslationService>();

            if (parameter is TranslationViewModel translationViewModel)
            {
                translationViewModel.CancellationTokenSource?.Cancel();
                translationViewModel.CancellationTokenSource = new CancellationTokenSource();
                await translationService.TranslateFile(translationViewModel, translationViewModel.CancellationTokenSource.Token);
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

            var translationService = App.GetService<ITranslationService>();

            if (parameter is TranslationListViewModel translationListViewModel)
            {
                var openFileDialog = new OpenFileDialog
                {
                    Multiselect = true,
                    Filter = "Subtitle Files (*.srt)|*.srt",
                    Title = "Select Subtitle Files"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var filePath in openFileDialog.FileNames)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string extension = Path.GetExtension(filePath); // Keeps the dot (e.g., .mp3)
                        var fileInfo = new FileInfo(filePath);

                        var encoding = Encoding.UTF8;
                        using(var reader = new StreamReader(filePath, Encoding.Default, true))
                        {
                            encoding = reader.CurrentEncoding;
                        }

                        var newTranslation = new TranslationViewModel
                        {
                            Name = fileName,
                            Path = Path.GetDirectoryName(filePath) ?? string.Empty,
                            Encoding = encoding,
                            Extension = extension,
                            FileSize = fileInfo.Length,
                            Status = TranslationStatus.New,
                            TranslationSetup = translationService.GetGlobalTranslationSetup(),
                            RequestCount = 0,
                            FailedRequestCount = 0,
                            Progress = 0
                        };

                        if (translationListViewModel.TranslationFiles is null)
                        {
                            translationListViewModel.TranslationFiles = new();
                        }

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            translationListViewModel.TranslationFiles.Add(newTranslation);
                        });
                    }
                }
                cacheService.CasheTranslations(translationListViewModel.TranslationFiles);
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
            var translationListViewModel = App.GetService<TranslationListViewModel>();

            if (parameter is TranslationViewModel translationViewModel)
            {
                translationViewModel.CancellationTokenSource?.Cancel();
                App.Current.Dispatcher.InvokeAsync(() =>
                {
                    translationViewModel.Status = TranslationStatus.Paused;
                });
                cacheService.CasheTranslations(translationListViewModel.TranslationFiles);
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
            var translationListViewModel = App.GetService<TranslationListViewModel>();

            if (parameter is TranslationViewModel translationViewModel)
            {
                translationListViewModel.TranslationFiles.Remove(translationViewModel);
                cacheService.CasheTranslations(translationListViewModel.TranslationFiles);
            }
        }
    }
}
