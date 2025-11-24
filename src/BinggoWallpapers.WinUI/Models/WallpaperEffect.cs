// Copyright (c) hippieZhou. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace BinggoWallpapers.WinUI.Models;

public partial class WallpaperEffect : ObservableObject
{
    [ObservableProperty]
    public partial float Exposure { get; set; } = 0;

    [ObservableProperty]
    public partial float Temperature { get; set; } = 0;

    [ObservableProperty]
    public partial float Tint { get; set; } = 0;

    [ObservableProperty]
    public partial float Contrast { get; set; } = 0;

    [ObservableProperty]
    public partial float Saturation { get; set; } = 1;

    [ObservableProperty]
    public partial float Blur { get; set; } = 0;

    [ObservableProperty]
    public partial float PixelScale { get; set; } = 1;
}
