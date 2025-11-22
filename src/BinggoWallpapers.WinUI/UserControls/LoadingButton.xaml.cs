using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinggoWallpapers.WinUI.UserControls;
public sealed partial class LoadingButton : UserControl
{
    public LoadingButton()
    {
        InitializeComponent();
    }

    [GeneratedDependencyProperty]
    public partial IAsyncRelayCommand? Command { get; set; }
}
