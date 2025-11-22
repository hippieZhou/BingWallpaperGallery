using BinggoWallpapers.WinUI.Selectors;
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinggoWallpapers.WinUI.UserControls;

/// <summary>
/// https://github.com/CommunityToolkit/Labs-Windows/blob/main/components/MarkdownTextBlock/src/MarkdownThemes.cs
/// </summary>
public sealed partial class MoreDetailsPanel : UserControl
{
    public MoreDetailsPanel()
    {
        InitializeComponent();
    }

    [GeneratedDependencyProperty]
    public partial string? Details { get; set; }

    partial void OnDetailsPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        var themeSelectorService = App.GetService<IThemeSelectorService>();
        var theme = themeSelectorService.Theme;
        switch (theme)
        {
            case ElementTheme.Default:
                break;
            case ElementTheme.Light:
                DetailsTextBlock.Config.Themes.CodeBlockForeground = new SolidColorBrush(Colors.Black);
                break;
            case ElementTheme.Dark:
                DetailsTextBlock.Config.Themes.CodeBlockForeground = new SolidColorBrush(Colors.White);
                break;
            default:
                break;
        }
    }

    private void ShadowGrid_Loaded(object sender, RoutedEventArgs e)
    {
        DialogShadow.Receivers.Add(sender as UIElement);
    }

    private void ShadowGrid_Tapped(object sender, TappedRoutedEventArgs e)
    {
        Hide();
    }

    private void OnCancel_Clicked(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    public void Hide()
    {
        Visibility = Visibility.Collapsed;
    }
}
