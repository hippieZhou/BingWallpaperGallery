using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace BinggoWallpapers.WinUI.Helpers;
public partial class FormattedDownloadProgressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double progress)
        {
            if (progress <= 0)
            {
                return "0 %";
            }

            return $"{Math.Round(progress, 1).ToString("0.0", CultureInfo.InvariantCulture)}%";
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
