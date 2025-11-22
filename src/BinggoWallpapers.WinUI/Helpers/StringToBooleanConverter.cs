// Copyright (c) hippieZhou. All rights reserved.

using Microsoft.UI.Xaml.Data;

namespace BinggoWallpapers.WinUI.Helpers;

public partial class StringToBooleanConverter : IValueConverter
{
    public StringToBooleanConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string parameterString && value is string valueString)
        {
            return parameterString.Equals(valueString, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue && boolValue && parameter is string parameterString)
        {
            return parameterString;
        }

        return string.Empty;
    }
}

