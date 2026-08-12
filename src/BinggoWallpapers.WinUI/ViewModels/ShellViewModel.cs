// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Notifications;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace BinggoWallpapers.WinUI.ViewModels;

public partial class ShellViewModel(
    INavigationService navigationService,
    INavigationViewService navigationViewService,
    IInAppNotificationService inAppNotificationService) : ObservableRecipient
{
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

    [RelayCommand]
    private void OnBackRequested()
    {
        navigationService.GoBack();
    }
}
