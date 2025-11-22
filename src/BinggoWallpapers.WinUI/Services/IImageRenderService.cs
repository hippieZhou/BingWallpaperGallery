// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Models;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace BinggoWallpapers.WinUI.Services;

/// <summary>
/// Mockup渲染服务，负责处理设备mockup和用户图片的绘制逻辑
/// </summary>
public interface IImageRenderService
{
    /// <summary>
    /// 计算mockup图片在画布中的显示区域
    /// </summary>
    Rect CalculateMockupRect(Size canvasSize, Size mockupImageSize);

    /// <summary>
    /// 计算屏幕区域在画布中的实际坐标
    /// </summary>
    Rect CalculateScreenRect(Rect mockupRect, DeviceConfiguration deviceConfig);

    /// <summary>
    /// 计算用户图片在屏幕区域内的最佳填充区域
    /// </summary>
    Rect CalculateUserImageRect(Rect screenRect, Size userImageSize, double screenAspectRatio);

    /// <summary>
    /// 绘制mockup图片
    /// </summary>
    void DrawMockup(CanvasDrawingSession session, CanvasBitmap mockupImage, Rect mockupRect);

    /// <summary>
    /// 绘制用户图片到屏幕区域
    /// </summary>
    void DrawUserImageOnScreen(CanvasDrawingSession session, CanvasBitmap userImage, Rect screenRect, Rect imageDrawRect,
        (float contrast, float exposure, float tint, float temperature, float saturation, float blur, float pixelScale) effect);

    /// <summary>
    /// 绘制屏幕边框（调试用）
    /// </summary>
    void DrawScreenBorder(CanvasDrawingSession session, Rect screenRect, bool visible = false);

    /// <summary>
    /// 清空绘制资源
    /// </summary>
    /// <param name="session"></param>
    void CleanUp(CanvasDrawingSession session);
}
