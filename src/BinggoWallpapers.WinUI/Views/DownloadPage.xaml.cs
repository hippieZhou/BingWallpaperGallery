using BinggoWallpapers.WinUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Views;

public sealed partial class DownloadPage : Page
{
    public DownloadViewModel ViewModel { get; }
    public DownloadPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<DownloadViewModel>();
    }

    private void OnDeleteDownloadCommand(object sender, RoutedEventArgs _)
    {
        if (sender is MenuFlyoutItem item)
        {
            var parameter = item.CommandParameter;
            ViewModel.DeleteDownloadCommand.Execute(parameter);
        }
    }

    private void OnCancelDownloadCommand(object sender, RoutedEventArgs _)
    {
        if (sender is MenuFlyoutItem item)
        {
            var parameter = item.CommandParameter;
            ViewModel.CancelDownloadCommand.Execute(parameter);
        }
    }

    private void OnRetryDownloadCommand(object sender, RoutedEventArgs _)
    {
        if (sender is MenuFlyoutItem item)
        {
            var parameter = item.CommandParameter;
            ViewModel.RetryDownloadCommand.Execute(parameter);
        }
    }

    private void OnViewOriginalImageCommand(object sender, RoutedEventArgs _)
    {
        if (sender is MenuFlyoutItem item)
        {
            var parameter = item.CommandParameter;
            ViewModel.ViewOriginalImageCommand.Execute(parameter);
        }
    }
}
