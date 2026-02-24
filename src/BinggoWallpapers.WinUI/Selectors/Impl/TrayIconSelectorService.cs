// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.Views.TrayIcon;
using CommunityToolkit.WinUI;
using DesktopFlyouts;
using Windows.ApplicationModel;

namespace BinggoWallpapers.WinUI.Selectors.Impl;

public class TrayIconSelectorService(
    DefaultTrayIconFlyout defaultTrayIconFlyout,
    DefaultTrayIconMeunFlyout defaultTrayIconMeunFlyout,
    ILocalSettingsService localSettingsService) :
    SelectorService(localSettingsService), ITrayIconSelectorService
{
    private SystemTrayIcon? _trayIcon;
    protected override string SettingsKey => "TrayIconEnabled";

    public bool IsEnabled { get; private set; } = false;

    public async Task InitializeAsync()
    {
        IsEnabled = await ReadFromSettingsAsync(false);
    }

    public async Task ToggleAsync(bool value)
    {
        IsEnabled = value;

        await SetRequestedTrayIconAsync();
        await SaveInSettingsAsync(IsEnabled);
    }

    public async Task SetRequestedTrayIconAsync()
    {
        if (IsEnabled)
        {
            await Register();
        }
        else
        {
            UnRegister();
        }
    }

    private async Task Register()
    {
        await App.MainWindow.DispatcherQueue.EnqueueAsync(() =>
        {
            _trayIcon ??= new SystemTrayIcon(
                Path.Combine(Package.Current.InstalledLocation.Path, "Assets\\WindowIcon.ico"),
                Models.AppInfo.AppTitle,
                Guid.Parse("28DE460A-8BD6-4539-A406-5F685584FD4D"));
            _trayIcon.LeftClicked += (sender, e) =>
            {
                if (defaultTrayIconFlyout.IsOpen)
                {
                    defaultTrayIconFlyout.Hide();
                    defaultTrayIconFlyout.ViewModel.LoadedCancelCommand.Execute(null);
                }
                else
                {
                    defaultTrayIconFlyout.Show();
                    defaultTrayIconFlyout.ViewModel.LoadedCommand.Execute(null);
                }
            };
            _trayIcon.RightClicked += (sender, e) =>
            {
                if (defaultTrayIconMeunFlyout.IsOpen)
                {
                    defaultTrayIconMeunFlyout.Hide();
                    defaultTrayIconMeunFlyout.ViewModel.LoadedCancelCommand.Execute(null);
                }
                else
                {
                    defaultTrayIconMeunFlyout.Show(e.Point);
                    defaultTrayIconMeunFlyout.ViewModel.LoadedCommand.Execute(null);
                }
            };
            _trayIcon.Show();
        });
    }
 
    private void UnRegister()
    {
        if(_trayIcon is null)
        {
            return;
        }

        _trayIcon.IsVisible = false;
        _trayIcon.Destroy();
    }
}
