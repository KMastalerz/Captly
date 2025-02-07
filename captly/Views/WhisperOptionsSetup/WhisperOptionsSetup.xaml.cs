using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace captly.Views.WhisperOptionsSetup;
/// <summary>
/// Interaction logic for WhisperOptionsSetup.xaml
/// </summary>
public partial class WhisperOptionsSetup : UserControl
{

    public WhisperOptionsSetup()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<WhisperOptionsSetupViewModel>();
    }
}
