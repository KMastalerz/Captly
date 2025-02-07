using captly.Views.Transcription;
using captly.Views.Translation;
using captly.Views.WhisperOptionsSetup;
using System.Windows.Input;

namespace captly;

public class MainWindowCommands
{
    public class OpenTranslationCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.View = new TranslationList();
            }
        }
    }

    public class OpenTranscriptionCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.View = new TranscriptionList();
            }
        }
    }

    public class OpenWhisperOptionsSetupCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.View = new WhisperOptionsSetup();
            }
        }
    }
}
