using captly.Enums;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace captly.Converters;
internal class TranslationStatusConverter : IValueConverter
{
    // Specify which statuses map to visible
    public TranslationStatus[] VisibleStatuses { get; set; } = Array.Empty<TranslationStatus>();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TranslationStatus status)
        {
            return Array.Exists(VisibleStatuses, s => s == status) ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented.");
    }
}
