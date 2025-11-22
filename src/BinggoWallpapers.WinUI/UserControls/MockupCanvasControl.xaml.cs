// Copyright (c) hippieZhou. All rights reserved.

using System.Windows.Input;
using BinggoWallpapers.WinUI.Models;
using BinggoWallpapers.WinUI.Services;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.UserControls;

public sealed partial class MockupCanvasControl : UserControl
{
    private readonly DeviceConfiguration _deviceConfiguration;
    private readonly IImageRenderService _renderService;
    private readonly ILogger<MockupCanvasControl> _logger;

    public MockupCanvasControl()
    {
        InitializeComponent();
        _deviceConfiguration = DeviceConfiguration.CreateSurfaceBook15();
        _renderService = App.GetService<IImageRenderService>();
        _logger = App.GetService<ILogger<MockupCanvasControl>>();

        Configuration = _deviceConfiguration;
    }

    public CanvasControl GetCanvasControl() => Canvas;

    #region DependencyProperties

    [GeneratedDependencyProperty]
    public partial ICommand? CreateResourcesCommand { get; set; }

    [GeneratedDependencyProperty]
    public partial CanvasBitmap? MockupImage { get; set; }

    [GeneratedDependencyProperty]
    public partial CanvasBitmap? WallpaperImage { get; set; }

    [GeneratedDependencyProperty]
    public partial DeviceConfiguration? Configuration { get; set; }

    [GeneratedDependencyProperty]
    public partial float ExposureAmount { get; set; }

    [GeneratedDependencyProperty]
    public partial float TemperatureAmount { get; set; }

    [GeneratedDependencyProperty]
    public partial float TintAmount { get; set; }

    [GeneratedDependencyProperty]
    public partial float ContrastAmount { get; set; }

    [GeneratedDependencyProperty]
    public partial float SaturationAmount { get; set; }

    [GeneratedDependencyProperty]
    public partial float BlurAmount { get; set; }

    [GeneratedDependencyProperty]
    public partial float Pixelation { get; set; }

    [GeneratedDependencyProperty]
    public partial bool IsInitialized { get; set; }

    [GeneratedDependencyProperty]
    public partial bool ShowScreenBorder { get; set; }

    #endregion

    #region OnPropertyChanged

    partial void OnIsInitializedPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        PR.Visibility = e.NewValue is true ? Visibility.Collapsed : Visibility.Visible;
        Canvas.Invalidate();
    }

    partial void OnExposureAmountPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Canvas.Invalidate();
    }

    partial void OnTemperatureAmountPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Canvas.Invalidate();
    }

    partial void OnTintAmountPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Canvas.Invalidate();
    }

    partial void OnContrastAmountPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Canvas.Invalidate();
    }

    partial void OnSaturationAmountPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Canvas.Invalidate();
    }

    partial void OnBlurAmountPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Canvas.Invalidate();
    }

    partial void OnPixelationPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Canvas.Invalidate();
    }

    #endregion

    private void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        CreateResourcesCommand?.Execute(sender);
        Canvas.Invalidate();
    }

    private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (IsInitialized is false || MockupImage is null)
        {
            return;
        }

        var session = args.DrawingSession;
        var canvasSize = sender.Size;

        try
        {
            var mockupRect = _renderService.CalculateMockupRect(canvasSize, MockupImage.Size);
            _renderService.DrawMockup(session, MockupImage, mockupRect);

            if (WallpaperImage is not null)
            {
                var screenRect = _renderService.CalculateScreenRect(mockupRect, _deviceConfiguration);
                var imageDrawRect = _renderService.CalculateUserImageRect(
                    screenRect,
                    WallpaperImage.Size,
                    _deviceConfiguration.ScreenAspectRatio);

                _renderService.DrawUserImageOnScreen(session, WallpaperImage, screenRect, imageDrawRect,
                    (contrast: ContrastAmount,
                    exposure: ExposureAmount,
                    tint: TintAmount,
                    temperature: TemperatureAmount,
                    saturation: SaturationAmount,
                    blur: BlurAmount,
                    pixelScale: Pixelation));
            }

            if (ShowScreenBorder)
            {
                var screenRect = _renderService.CalculateScreenRect(mockupRect, _deviceConfiguration);
                _renderService.DrawScreenBorder(session, screenRect, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"绘制Mockup画布时发生错误:{ex.Message}");
        }
    }
}
