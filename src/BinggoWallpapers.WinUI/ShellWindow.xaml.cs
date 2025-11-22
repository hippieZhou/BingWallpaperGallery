// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Helpers;
using BinggoWallpapers.WinUI.ViewModels;

namespace BinggoWallpapers.WinUI;

public sealed partial class ShellWindow : WindowEx
{
    public ShellViewModel ViewModel { get; } = App.GetService<ShellViewModel>();

    public ShellWindow()
    {
        InitializeComponent();
        this.CenterOnScreen();
        this.SetAppTitleBar(AppTitleBar, "Assets/WindowIcon.ico");
        ViewModel.Initialize(
            NavView,
            NavFrame,
            InAppNotification.NotificationQueue);
    }
}
