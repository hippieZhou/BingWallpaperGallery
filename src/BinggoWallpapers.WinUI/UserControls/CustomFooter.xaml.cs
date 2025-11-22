// Copyright (c) hippieZhou. All rights reserved.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinggoWallpapers.WinUI.UserControls;

public sealed partial class CustomFooter : UserControl
{
    public CustomFooter()
    {
        InitializeComponent();
    }

    [GeneratedDependencyProperty]
    public partial bool IsEmpty { get; set; }

    [GeneratedDependencyProperty]
    public partial bool IsLoading { get; set; }
}
