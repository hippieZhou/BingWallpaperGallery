// Copyright (c) hippieZhou. All rights reserved.

using BingWallpaperGallery.Core.DTOs;
using BingWallpaperGallery.WinUI.Models;
using BingWallpaperGallery.WinUI.ViewModels;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;

namespace BingWallpaperGallery.WinUI.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel { get; }

    public HomePage()
    {
        InitializeComponent();
        ViewModel = App.GetService<HomeViewModel>();
    }

    public static bool IsEmpty(IncrementalLoadingCollection<WallpaperInfoSource, WallpaperInfoDto>? items)
    {
        return items is null || !items.Any();
    }
}
