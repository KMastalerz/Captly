using captly.Views.Transcription;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace captly.Views.Translation;

/// <summary>
/// Interaction logic for TranslationList.xaml
/// </summary>
public partial class TranslationList : UserControl
{
    public TranslationList()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<TranslationListViewModel>();
    }
}
