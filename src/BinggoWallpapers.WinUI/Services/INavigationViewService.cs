// Copyright (c) hippieZhou. All rights reserved.

using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Services;

public interface INavigationViewService
{
    IList<object> MenuItems
    {
        get;
    }

    object SettingsItem
    {
        get;
    }

    void Initialize(NavigationView navigationView);

    void UnregisterEvents();

    NavigationViewItem GetSelectedItem(Type pageType);
}
