using Microsoft.UI.Xaml.Data;

namespace BinggoWallpapers.WinUI.Helpers;
public partial class FormattedDownloadSpeedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double speed)
        {
            if (speed <= 0)
            {
                return "0 B/s";
            }

            var speedMB = speed / 1024.0 / 1024.0;
            var speedKB = speed / 1024.0;

            return speedMB >= 1 ? $"{speedMB:F2} MB/s" : $"{speedKB:F2} KB/s";
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
