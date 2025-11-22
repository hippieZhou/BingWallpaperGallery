// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<SettingsViewModel>();
    }
}
