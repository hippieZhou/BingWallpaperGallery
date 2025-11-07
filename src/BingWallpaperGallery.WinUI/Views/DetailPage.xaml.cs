// Copyright (c) hippieZhou. All rights reserved.

using System.Globalization;
using BingWallpaperGallery.WinUI.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace BingWallpaperGallery.WinUI.Views;

public sealed partial class DetailPage : Page
{
    public CultureInfo Culture { get; } 
    public DetailViewModel ViewModel { get; }

    public DetailPage()
    {
        InitializeComponent();
        Culture = CultureInfo.CurrentCulture;
        ViewModel = App.GetService<DetailViewModel>();
    }

    private void ToggleEditState()
    {
        WallpaperView.IsPaneOpen = !WallpaperView.IsPaneOpen;
    }
}
