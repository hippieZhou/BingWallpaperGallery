using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace BinggoWallpapers.WinUI.Helpers;
public partial class EstimatedTimeRemainingVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is TimeSpan estimatedTimeRemaining
            ? estimatedTimeRemaining == TimeSpan.Zero ? Visibility.Collapsed : Visibility.Visible
            : value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
