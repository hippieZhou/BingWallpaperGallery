// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Helpers;
using BinggoWallpapers.WinUI.Services;
using Microsoft.UI.Xaml;

namespace BinggoWallpapers.WinUI.Selectors.Impl;

public class ThemeSelectorService(ILocalSettingsService localSettingsService) :
    SelectorService(localSettingsService), IThemeSelectorService
{
    public ElementTheme Theme { get; private set; } = ElementTheme.Default;
    protected override string SettingsKey => "AppTheme";

    public async Task InitializeAsync()
    {
        var themeName = await ReadFromSettingsAsync(SettingsKey);
        Theme = Enum.TryParse(themeName, out ElementTheme cacheTheme) ? cacheTheme : ElementTheme.Default;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        await SaveInSettingsAsync(theme.ToString());
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }
}
