using captly.Views.Transcription;
using captly.Views.Translation;
using System.Windows.Input;

namespace captly;

public class MainWindowCommands
{
    public class OpenDrawerCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.IsDrawerOpen = true;
            }
        }
    }

    public class OpenTranslationCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (parameter is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.IsDrawerOpen = false;
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
                mainWindowViewModel.IsDrawerOpen = false;
                mainWindowViewModel.View = new TranscriptionList();
            }
        }
    }
}
