using System.Windows;
using System.Windows.Controls;
using captly.Interfaces;

namespace captly.Services;
internal class DisplayService(MainWindowViewModel mainWindowViewModel) : IDisplayService
{
    public void OpenDialog(UserControl userControl)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            mainWindowViewModel.Dialog = userControl;
        });
    }

    public void CloseDialog()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            mainWindowViewModel.Dialog = null;
        });
    }

    public void OpenWindow(UserControl userControl)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            mainWindowViewModel.View = userControl;
        });
    }

    public void CloseWindow(UserControl userControl)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            mainWindowViewModel.View = null;
        });
    }
}
