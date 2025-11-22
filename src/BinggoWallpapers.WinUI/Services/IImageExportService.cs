// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace BinggoWallpapers.WinUI.Services;

/// <summary>
/// 图片导出服务，负责将Canvas内容导出为图片文件
/// </summary>
public interface IImageExportService
{
    /// <summary>
    /// 导出Canvas内容为图片文件
    /// </summary>
    Task<bool> ExportCanvasAsync(
        CanvasControl canvasControl,
        CanvasBitmap mockupImage,
        CanvasBitmap userImage,
        DeviceConfiguration deviceConfig,
        (float contrast, float exposure, float tint, float temperature, float saturation, float blur, float pixelScale) effect,
        float scaleFactor = 2.0f);
}
