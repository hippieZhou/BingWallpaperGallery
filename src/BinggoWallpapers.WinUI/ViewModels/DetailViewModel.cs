// Copyright (c) hippieZhou. All rights reserved.

using System.Collections.ObjectModel;
using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Notifications;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace BinggoWallpapers.WinUI.ViewModels;

public partial class DetailViewModel(
    IImageExportService exportService,
    IDownloadService downloadService,
    IInAppNotificationService inAppNotificationService,
    IManagementService managementService,
    ILogger<DetailViewModel> logger) : ObservableRecipient, INavigationAware
{
    [ObservableProperty]
    public partial ObservableCollection<ResolutionInfoDto> SupportedResolutions { get; set; }

    [ObservableProperty]
    public partial CanvasBitmap MockupImage { get; set; }

    [ObservableProperty]
    public partial CanvasBitmap WallpaperImage { get; set; }

    [ObservableProperty]
    public partial WallpaperInfoDto Wallpaper { get; set; }

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

    [ObservableProperty]
    public partial bool IsInitialized { get; set; }

    [ObservableProperty]
    public partial bool IsDownloading { get; set; }

    [ObservableProperty]
    public partial bool IsExporting { get; set; }

    public void OnNavigatedFrom()
    {
        Wallpaper = null;

        MockupImage?.Dispose();
        MockupImage = null;
        WallpaperImage?.Dispose();
        WallpaperImage = null;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is WallpaperInfoDto wallpaper)
        {
            Wallpaper = wallpaper;
            logger.LogInformation($"导航到壁纸详情页: {wallpaper.Title}");
        }
        else
        {
            inAppNotificationService.ShowError(
                message: "无效的壁纸信息",
                title: "导航错误",
                details: "无法获取壁纸详情信息"
            );
            logger.LogWarning("导航到壁纸详情页时参数无效");
        }
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task OnLoaded(CancellationToken cancellationToken = default)
    {
        if (SupportedResolutions is not null && SupportedResolutions.Any())
        {
            return;
        }

        var resultions = await managementService.GetSupportedResolutionsAsync(cancellationToken);
        SupportedResolutions = new ObservableCollection<ResolutionInfoDto>(resultions);
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task OnCreateResources(CanvasControl canvasControl, CancellationToken cancellationToken = default)
    {
        if (canvasControl is null || Wallpaper is null)
        {
            return;
        }

        try
        {
            IsInitialized = false;
            logger.LogInformation($"开始加载壁纸预览: {Wallpaper.Title}");

            MockupImage?.Dispose();
            MockupImage = await LoadImageAsync("ms-appx:///Assets/Mockups/microsoft-surface-book.png", canvasControl, logger);

            WallpaperImage?.Dispose();
            WallpaperImage = await LoadImageAsync(Wallpaper.Url, canvasControl, logger);
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            IsInitialized = false;
            logger.LogError(ex, $"加载壁纸预览失败: {ex.Message}");
        }
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task<string> OnViewMoreDetails(CancellationToken cancellationToken = default)
    {
        if (Wallpaper is null)
        {
            return string.Empty;
        }

        var jsonDetails = await managementService.GetMoreDetailsAsync(Wallpaper.Id, cancellationToken);
        return $@"
```json
{jsonDetails}
```
";
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task OnDownloadWallpaper(ResolutionInfoDto resolution, CancellationToken cancellation = default)
    {
        if (Wallpaper == null)
        {
            return;
        }

        try
        {
            IsDownloading = true;
            inAppNotificationService.ShowInfo($"开始后台下载壁纸: {Wallpaper.Title}");

            logger.LogInformation($"开始后台下载壁纸: {Wallpaper.Title}");

            var downloadId = await downloadService.DownloadAsync(Wallpaper, resolution, cancellation);
            logger.LogInformation($"后台下载壁纸的任务ID: {downloadId}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"开始后台下载壁纸失败: {ex.Message}");
        }
        finally
        {
            IsDownloading = false;
        }
    }

    [RelayCommand]
    private void OnResetEffects()
    {
        Exposure = Blur = Tint = Temperature = Contrast = 0;
        Saturation = PixelScale = 1;
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false, FlowExceptionsToTaskScheduler = true)]
    private async Task OnExportWallpaper(MockupCanvasControl mockupCanvasControl, CancellationToken cancellationToken = default)
    {
        if (Wallpaper == null)
        {
            return;
        }

        try
        {
            IsExporting = true;
            var canvasControl = mockupCanvasControl.GetCanvasControl();
            var mockup = mockupCanvasControl.MockupImage;
            var image = mockupCanvasControl.WallpaperImage;
            var config = mockupCanvasControl.Configuration;
            var success = await exportService.ExportCanvasAsync(
                 canvasControl,
                 mockup,
                 image,
                 config!,
                 (contrast: Contrast,
                  exposure: Exposure,
                  tint: Tint,
                  temperature: Temperature,
                  saturation: Saturation,
                  blur: Blur,
                  pixelScale: PixelScale));
            if (success)
            {
                inAppNotificationService.ShowSuccess($"导出壁纸: {Wallpaper.Title}");
            }
        }
        catch (Exception ex)
        {
            inAppNotificationService.ShowError($"导出壁纸失败: {ex.Message}");
        }
        finally
        {
            IsExporting = false;
        }
    }

    #region Private Methods
    private static async Task<CanvasBitmap> LoadImageAsync(string uri, CanvasControl canvasControl, ILogger<DetailViewModel> logger)
    {
        try
        {
            return await CanvasBitmap.LoadAsync(canvasControl, new Uri(uri));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"加载图片失败: {ex.Message}");
            return null;
        }
    }
    #endregion
}
