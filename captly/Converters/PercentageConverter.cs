using System.Globalization;
using System.Windows.Data;

namespace captly.Converters;
internal class PercentageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            // Format the value to a whole number percentage
            return $"{Math.Round(doubleValue)}%";
        }

        return "0%";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Two-way binding is not supported.");
    }
}
