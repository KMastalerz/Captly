using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace captly.Views.Transcription;

/// <summary>
/// Interaction logic for TranscriptionList.xaml
/// </summary>
public partial class TranscriptionList : UserControl
{
    public TranscriptionList()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<TranscriptionListViewModel>();
    }
}
