// Copyright (c) hippieZhou. All rights reserved.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace BinggoWallpapers.WinUI.Services;

public interface INavigationService
{
    event NavigatedEventHandler Navigated;

    bool CanGoBack
    {
        get;
    }

    Frame Frame
    {
        get; set;
    }

    bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false);

    bool NavigateTo<T>(object parameter = null, bool clearNavigation = false);

    bool GoBack();
}
