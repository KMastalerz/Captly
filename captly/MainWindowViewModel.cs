using captly.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static captly.MainWindowCommands;

namespace captly;

public class MainWindowViewModel : BaseViewModel
{
    public MainWindowViewModel()
    {
        OpenDrawerCommand = new OpenDrawerCommand();
        OpenTranslationCommand = new OpenTranslationCommand();
        OpenTranscriptionCommand = new OpenTranscriptionCommand();
    }

    private UserControl? view = null;
    public UserControl? View
    {
        get => view;
        set => SetProperty(ref view, value);
    }
    private UserControl? dialog = null;
    public UserControl? Dialog
    {
        get => dialog;
        set => SetProperty(ref dialog, value);
    }

    private string snackbarMessage = string.Empty;
    public string SnackbarMessage
    {
        get => snackbarMessage;
        set => SetProperty(ref snackbarMessage, value);
    }

    private bool isSnackbarActive = false;
    public bool IsSnackbarActive
    {
        get => isSnackbarActive;
        set => SetProperty(ref isSnackbarActive, value);
    }

    private bool isDrawerOpen = false;
    public bool IsDrawerOpen
    {
        get => isDrawerOpen;
        set => SetProperty(ref isDrawerOpen, value);
    }

    private Style? snackbarStyle;
    public Style? SnackbarStyle
    {
        get => snackbarStyle;
        set => SetProperty(ref snackbarStyle, value);
    }

    public ICommand OpenDrawerCommand { get; }
    public ICommand OpenTranslationCommand { get; }
    public ICommand OpenTranscriptionCommand { get; }
}
