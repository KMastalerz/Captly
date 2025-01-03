using System.Windows.Controls;

namespace captly.Interfaces;
public interface IDisplayService
{
    void CloseDialog();
    void CloseWindow(UserControl userControl);
    void OpenDialog(UserControl userControl);
    void OpenWindow(UserControl userControl);
}