// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Notifications;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.BadgeNotifications;

namespace BinggoWallpapers.WinUI.ViewModels;

public partial class ShellViewModel(
    IMemoryCache memoryCache,
    INavigationService navigationService,
    INavigationViewService navigationViewService,
    IManagementService managementService,
    IInAppNotificationService inAppNotificationService) : ObservableRecipient
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    public StackedNotificationsBehavior NotificationQueue;

    [ObservableProperty]
    public partial object Selected { get; set; }

    [ObservableProperty]
    public partial bool IsBackEnabled { get; set; }

    internal void Initialize(
        NavigationView navView,
        Frame navFrame,
        StackedNotificationsBehavior notificationQueue)
    {
        navigationViewService.Initialize(navView);
        navigationService.Frame = navFrame;
        navigationService.Navigated += OnNavigated;
        inAppNotificationService.NotificationQueue = notificationQueue;
        IsActive = true;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = navigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = navigationViewService.SettingsItem;
            return;
        }

        var selectedItem = navigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }

    [RelayCommand(IncludeCancelCommand = true, FlowExceptionsToTaskScheduler = true, AllowConcurrentExecutions = false)]
    private async Task OnFetchRequest(CancellationToken cancellationToken)
    {
        try
        {
            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.Activity);

            await Task.Run(async () =>
            {
                await managementService.RunCollectionAsync(cancellationToken);
                if (_memoryCache is MemoryCache memoryCache)
                {
                    memoryCache.Clear();
                }
            }, cancellationToken);
            inAppNotificationService.ShowSuccess("所有壁纸信息收集完成！");
        }
        catch (Exception ex)
        {
            inAppNotificationService.ShowError(ex.Message);
        }
        finally
        {
            BadgeNotificationManager.Current.ClearBadge();
        }
    }

    [RelayCommand]
    private void OnBackRequested()
    {
        navigationService.GoBack();
    }
}
