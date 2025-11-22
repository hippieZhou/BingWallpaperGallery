using Microsoft.UI.Xaml.Data;

namespace BinggoWallpapers.WinUI.Helpers;
public partial class FormattedFileSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is long totalBytes)
        {
            if (totalBytes <= 0)
            {
                return "未知大小";
            }

            var size = totalBytes / 1024.0 / 1024.0;
            return size >= 1 ? $"{size:F2} MB" : $"{totalBytes / 1024.0:F2} KB";
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
