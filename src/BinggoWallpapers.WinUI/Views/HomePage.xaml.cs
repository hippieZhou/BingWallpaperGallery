// Copyright (c) hippieZhou. All rights reserved.

using System.Numerics;
using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.WinUI.Models;
using BinggoWallpapers.WinUI.ViewModels;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel { get; }

    public HomePage()
    {
        InitializeComponent();
        ViewModel = App.GetService<HomeViewModel>();
        RefreshButton.Translation += new Vector3(0, 0, 32);
    }

    public static bool IsEmpty(IncrementalLoadingCollection<WallpaperInfoSource, WallpaperInfoDto>? items)
    {
        return items is null || !items.Any();
    }
}
