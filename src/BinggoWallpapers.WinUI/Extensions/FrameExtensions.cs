// Copyright (c) hippieZhou. All rights reserved.

using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Extensions;

public static class FrameExtensions
{
    public static object GetPageViewModel(this Frame frame) => frame?.Content?.GetType().GetProperty("ViewModel")?.GetValue(frame.Content, null);
}
