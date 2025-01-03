﻿using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace captly.Views;
/// <summary>
/// Interaction logic for TranslateOptions.xaml
/// </summary>
public partial class TranslateOptions : UserControl
{
    public TranslateOptions()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<TranslateOptionsViewModel>();
    }
}
