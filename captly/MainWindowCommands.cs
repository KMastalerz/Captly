using captly.Views;
using System.Windows.Input;

namespace captly;
internal class MainWindowCommands
{
    public class OpenDrawer : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if(parameter is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.IsDrawerOpen = true;
            }
        }
    }

    public class OpenTranslation : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if(parameter is MainWindowViewModel mainWindowViewModel)
            {
                mainWindowViewModel.IsDrawerOpen = false;
                mainWindowViewModel.View = new Translate();
            }
        }
    }

}
