using System.Text.RegularExpressions;
using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Extensions;
using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Selectors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopFlyouts;
using Microsoft.UI.Xaml;

namespace BinggoWallpapers.WinUI.Views.TrayIcon;

public sealed partial class DefaultTrayIconFlyout : DesktopFlyout
{
    public DefaultTrayIconFlyoutViewModel ViewModel { get; }
    public DefaultTrayIconFlyout(DefaultTrayIconFlyoutViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    public static string ConvertToMobileResolution(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        const string MobilePortraitSuffix = "_1080x1920.jpg";

        var resolutionPatterns = new[]
        {
            ResolutionCode.UHD4K.GetSuffix(),      // _UHD.jpg
            ResolutionCode.HD.GetSuffix(),        // _1920x1200.jpg
            ResolutionCode.FullHD.GetSuffix(),    // _1920x1080.jpg
            ResolutionCode.Standard.GetSuffix()   // _1366x768.jpg
        };

        foreach (var pattern in resolutionPatterns)
        {
            if (url.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return url.Replace(pattern, MobilePortraitSuffix, StringComparison.OrdinalIgnoreCase);
            }
        }

        var regex = new Regex(@"_(\d+x\d+|UHD)\.jpg", RegexOptions.IgnoreCase);
        var match = regex.Match(url);
        if (match.Success)
        {
            return url.Substring(0, match.Index) + MobilePortraitSuffix;
        }

        if (url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
            !url.Contains("_", StringComparison.OrdinalIgnoreCase))
        {
            return url.Replace(".jpg", MobilePortraitSuffix, StringComparison.OrdinalIgnoreCase);
        }

        return url;
    }

    private void OnHome(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow.Visible == false)
        {
            App.MainWindow.Show();
            App.MainWindow.Activate();
        }
    }
}

public partial class DefaultTrayIconFlyoutViewModel(
    IManagementService management,
    IMarketSelectorService marketSelector) : ObservableObject
{
    [ObservableProperty]
    public partial WallpaperInfoDto? Wallpaper { get; set; }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task OnLoaded(CancellationToken cancellationToken = default)
    {
        var market = marketSelector.Market;
        Wallpaper = await management.GetLatestAsync(market, cancellationToken);
    }
}
