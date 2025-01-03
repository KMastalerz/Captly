using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace captly.Views;
/// <summary>
/// Interaction logic for Translate.xaml
/// </summary>
public partial class Translate : UserControl
{
    public Translate()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<TranslateViewModel>();
    }
}
