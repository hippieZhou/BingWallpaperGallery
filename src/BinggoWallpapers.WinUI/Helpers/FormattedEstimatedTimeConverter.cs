using Microsoft.UI.Xaml.Data;

namespace BinggoWallpapers.WinUI.Helpers;
public partial class FormattedEstimatedTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is TimeSpan estimatedTimeRemaining)
        {
            if (estimatedTimeRemaining == TimeSpan.Zero)
            {
                return "未知";
            }

            if (estimatedTimeRemaining.TotalHours >= 1)
            {
                return $"{estimatedTimeRemaining.Hours:D2}:{estimatedTimeRemaining.Minutes:D2}:{estimatedTimeRemaining.Seconds:D2}";
            }

            if (estimatedTimeRemaining.TotalMinutes >= 1)
            {
                return $"{estimatedTimeRemaining.Minutes:D2}:{estimatedTimeRemaining.Seconds:D2}";
            }

            return $"{estimatedTimeRemaining.Seconds} 秒";

        }
        else
        {
            return value;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
