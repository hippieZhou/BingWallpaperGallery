// Copyright (c) hippieZhou. All rights reserved.

using System.Collections.Specialized;
using System.Web;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.ViewModels;
using Microsoft.Windows.AppNotifications;

namespace BinggoWallpapers.WinUI.Notifications.Impl;

/// <summary>
/// 应用内消息服务实现
/// </summary>
public class AppNotificationService : IAppNotificationService
{
    private readonly INavigationService _navigationService;

    public AppNotificationService(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Initialize();
    }

    ~AppNotificationService()
    {
        Unregister();
    }

    private void Initialize()
    {
        AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

        AppNotificationManager.Default.Register();
    }

    private void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        // TODO: Handle notification invocations when your app is already running.

        //// // Navigate to a specific page based on the notification arguments.
        if (ParseArguments(args.Argument)["action"] == "Settings")
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() => _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!));
        }

        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            App.MainWindow.ShowMessageDialogAsync("TODO: Handle notification invocations when your app is already running.", "Notification Invoked");

            App.MainWindow.BringToFront();
        });
    }

    public bool Show(string payload)
    {
        var appNotification = new AppNotification(payload);

        AppNotificationManager.Default.Show(appNotification);

        return appNotification.Id != 0;
    }

    public bool Show(AppNotification payload)
    {
        AppNotificationManager.Default.Show(payload);

        return payload.Id != 0;
    }

    public NameValueCollection ParseArguments(string arguments)
    {
        return HttpUtility.ParseQueryString(arguments);
    }

    private void Unregister()
    {
        AppNotificationManager.Default.Unregister();
    }
}
