using BinggoWallpapers.Core.DTOs;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.UserControls;

public sealed partial class CustomCard : UserControl
{
    public CustomCard()
    {
        InitializeComponent();
    }

    [GeneratedDependencyProperty]
    public partial WallpaperInfoDto? Model { get; set; }
}
