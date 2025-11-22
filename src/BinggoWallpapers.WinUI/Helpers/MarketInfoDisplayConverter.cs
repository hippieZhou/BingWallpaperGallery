using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.WinUI.Selectors;
using BinggoWallpapers.WinUI.Selectors.Impl;
using Microsoft.UI.Xaml.Data;

namespace BinggoWallpapers.WinUI.Helpers;

public partial class MarketInfoDisplayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is MarketInfoDto market)
        {
            var service = App.GetService<ILanguageSelectorService>();
            var lang = service.Language;
            return string.Equals(lang, LanguageSelectorService.DefaultLanguage, StringComparison.OrdinalIgnoreCase) ?
               market.CN :
               market.EN;
        }
        else
        {
            return value?.ToString();
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
