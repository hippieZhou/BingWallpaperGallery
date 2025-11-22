// Copyright (c) hippieZhou. All rights reserved.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinggoWallpapers.WinUI.UserControls;

public sealed partial class CustomHeader : UserControl
{
    public CustomHeader()
    {
        InitializeComponent();
    }

    [GeneratedDependencyProperty]
    public partial object? Source { get; set; }

    [GeneratedDependencyProperty]
    public partial string? Subtitle { get; set; }

    [GeneratedDependencyProperty]
    public partial string? Title { get; set; }

    [GeneratedDependencyProperty]
    public partial string? Desc { get; set; }

    [GeneratedDependencyProperty]
    public partial object? ItemsSource { get; set; }

    [GeneratedDependencyProperty]
    public partial object? SelectedItem { get; set; }
}
