using System.Globalization;
using System.Windows.Data;

namespace captly.Convertes;

public class TimeSpanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss"); 
        }
        return "00:00:00";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (TimeSpan.TryParseExact(value.ToString(), @"hh\:mm\:ss", CultureInfo.InvariantCulture, out TimeSpan timeSpan))
        {
            return timeSpan;
        }
        return TimeSpan.Zero;
    }
}