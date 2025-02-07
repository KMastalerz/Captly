using System.Globalization;
using System.Windows.Data;
using System.Windows;
using captly.Constants;

namespace captly.Convertes;

public class StatusToTranscribeVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TranscriptionStatus transcriptionStatus)
        {
            return transcriptionStatus == TranscriptionStatus.Error || transcriptionStatus == TranscriptionStatus.New || transcriptionStatus == TranscriptionStatus.Cancelled || transcriptionStatus == TranscriptionStatus.Finished
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StatusToTranslateVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TranslationStatus translationStatus)
        {
            return translationStatus == TranslationStatus.Error || translationStatus == TranslationStatus.New || translationStatus == TranslationStatus.Paused || translationStatus == TranslationStatus.Finished
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StatusToStopVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TranscriptionStatus transcriptionStatus)
        {
            return transcriptionStatus == TranscriptionStatus.Transcribing
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else if (value is TranslationStatus translationStatus)
        {

            return translationStatus == TranslationStatus.Translating
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StatusToRemoveVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TranscriptionStatus transcriptionStatus)
        {
            return transcriptionStatus == TranscriptionStatus.Transcribing
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else if (value is TranslationStatus translationStatus)
        {
            return translationStatus == TranslationStatus.Translating
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StatusToOptionsVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TranscriptionStatus transcriptionStatus)
        {
            return transcriptionStatus == TranscriptionStatus.Transcribing
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else if (value is TranslationStatus translationStatus)
        {
            return translationStatus == TranslationStatus.Translating
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        else return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
