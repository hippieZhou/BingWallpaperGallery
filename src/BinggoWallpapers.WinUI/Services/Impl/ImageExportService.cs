// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace BinggoWallpapers.WinUI.Services.Impl;

public class ImageExportService(IImageRenderService renderService, ILogger<ImageExportService> logger) : IImageExportService
{
    public async Task<bool> ExportCanvasAsync(
        CanvasControl canvasControl,
        CanvasBitmap mockupImage,
        CanvasBitmap userImage,
        DeviceConfiguration deviceConfig,
        (float contrast, float exposure, float tint, float temperature, float saturation, float blur, float pixelScale) effect,
        float scaleFactor = 2.0f)
    {
        try
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                DefaultFileExtension = ".png",
                SuggestedFileName = $"{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            picker.FileTypeChoices.Add("PNG图片", new[] { ".png" });
            picker.FileTypeChoices.Add("JPEG图片", new[] { ".jpg", ".jpeg" });

            // 获取窗口句柄
            var hwnd = App.MainWindow.GetWindowHandle();
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file == null)
            {
                return false;
            }

            return await ExportCanvasToFileAsync(canvasControl, file, mockupImage, userImage, deviceConfig, effect, scaleFactor);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Export failed: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> ExportCanvasToFileAsync(
        CanvasControl canvasControl,
        StorageFile file,
        CanvasBitmap mockupImage,
        CanvasBitmap userImage,
        DeviceConfiguration deviceConfig,
        (float contrast, float exposure, float tint, float temperature, float saturation, float blur, float pixelScale) effect,
        float scaleFactor)
    {
        if (mockupImage == null)
        {
            return false;
        }

        try
        {
            var canvasSize = canvasControl.Size;
            var highResWidth = (float)(canvasSize.Width * scaleFactor);
            var highResHeight = (float)(canvasSize.Height * scaleFactor);

            // 创建离屏渲染目标
            using var renderTarget = new CanvasRenderTarget(canvasControl, highResWidth, highResHeight, 96 * scaleFactor);

            // 在渲染目标上绘制内容
            using (var session = renderTarget.CreateDrawingSession())
            {
                // 清除背景
                session.Clear(Windows.UI.Color.FromArgb(255, 255, 255, 255)); // 白色背景

                // 应用缩放变换
                session.Transform = System.Numerics.Matrix3x2.CreateScale(scaleFactor);

                // 重新绘制mockup和用户图片
                DrawMockupContent(session, canvasSize, mockupImage, userImage, deviceConfig, effect);
            }

            // 导出到文件
            using var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            await SaveRenderTargetToStreamAsync(renderTarget, stream, file.FileType);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Export to file failed: {ex.Message}");
            return false;
        }
    }

    private void DrawMockupContent(
        CanvasDrawingSession session,
        Windows.Foundation.Size canvasSize,
        CanvasBitmap mockupImage,
        CanvasBitmap userImage,
        DeviceConfiguration deviceConfig,
        (float contrast, float exposure, float tint, float temperature, float saturation, float blur, float pixelScale) effect)
    {
        // 计算mockup显示区域
        var mockupRect = renderService.CalculateMockupRect(canvasSize, mockupImage.Size);

        // 绘制mockup图片
        renderService.DrawMockup(session, mockupImage, mockupRect);

        // 如果有用户图片，绘制到屏幕区域
        if (userImage != null)
        {
            var screenRect = renderService.CalculateScreenRect(mockupRect, deviceConfig);
            var imageDrawRect = renderService.CalculateUserImageRect(
                screenRect,
                userImage.Size,
                deviceConfig.ScreenAspectRatio);

            renderService.DrawUserImageOnScreen(session, userImage, screenRect, imageDrawRect, effect);
        }
    }

    private static async Task SaveRenderTargetToStreamAsync(CanvasRenderTarget renderTarget, IRandomAccessStream stream, string fileType)
    {
        var format = fileType.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => BitmapEncoder.JpegEncoderId,
            ".png" => BitmapEncoder.PngEncoderId,
            _ => BitmapEncoder.PngEncoderId
        };

        // 获取像素数据
        var pixelBytes = renderTarget.GetPixelBytes();

        // 创建编码器
        var encoder = await BitmapEncoder.CreateAsync(format, stream);

        // 设置像素数据
        encoder.SetPixelData(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            renderTarget.SizeInPixels.Width,
            renderTarget.SizeInPixels.Height,
            renderTarget.Dpi,
            renderTarget.Dpi,
            pixelBytes);

        // 如果是JPEG，设置插值模式以提高质量
        if (format == BitmapEncoder.JpegEncoderId)
        {
            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
            // 注意：JPEG质量设置在某些WinUI版本中可能不可用，使用默认质量
        }

        await encoder.FlushAsync();
    }
}
