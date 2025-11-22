// Copyright (c) hippieZhou. All rights reserved.

using System.Numerics;
using BinggoWallpapers.WinUI.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.UI;
using Windows.Foundation;

namespace BinggoWallpapers.WinUI.Services.Impl;

public class ImageRenderService : IImageRenderService
{
    public Rect CalculateMockupRect(Size canvasSize, Size mockupImageSize)
    {
        // 计算保持宽高比的缩放
        var scale = Math.Min(canvasSize.Width / mockupImageSize.Width, canvasSize.Height / mockupImageSize.Height);
        var scaledWidth = mockupImageSize.Width * scale;
        var scaledHeight = mockupImageSize.Height * scale;

        // 居中显示
        var offsetX = (canvasSize.Width - scaledWidth) / 2;
        var offsetY = (canvasSize.Height - scaledHeight) / 2;

        return new Rect(offsetX, offsetY, scaledWidth, scaledHeight);
    }

    public Rect CalculateScreenRect(Rect mockupRect, DeviceConfiguration deviceConfig)
    {
        var screenLeft = mockupRect.X + mockupRect.Width * deviceConfig.ScreenTopLeft.X;
        var screenTop = mockupRect.Y + mockupRect.Height * deviceConfig.ScreenTopLeft.Y;
        var screenRight = mockupRect.X + mockupRect.Width * deviceConfig.ScreenBottomRight.X;
        var screenBottom = mockupRect.Y + mockupRect.Height * deviceConfig.ScreenBottomRight.Y;

        return new Rect(
            screenLeft,
            screenTop,
            screenRight - screenLeft,
            screenBottom - screenTop
        );
    }

    public Rect CalculateUserImageRect(Rect screenRect, Size userImageSize, double screenAspectRatio)
    {
        var userImageAspectRatio = userImageSize.Width / userImageSize.Height;

        if (userImageAspectRatio > screenAspectRatio)
        {
            // 用户图片比屏幕更宽，按高度填充，裁剪左右
            var drawHeight = screenRect.Height;
            var drawWidth = drawHeight * userImageAspectRatio;
            var offsetX = (screenRect.Width - drawWidth) / 2;

            return new Rect(
                screenRect.X + offsetX,
                screenRect.Y,
                drawWidth,
                drawHeight
            );
        }
        else
        {
            // 用户图片比屏幕更高，按宽度填充，裁剪上下
            var drawWidth = screenRect.Width;
            var drawHeight = drawWidth / userImageAspectRatio;
            var offsetY = (screenRect.Height - drawHeight) / 2;

            return new Rect(
                screenRect.X,
                screenRect.Y + offsetY,
                drawWidth,
                drawHeight
            );
        }
    }

    public void DrawMockup(CanvasDrawingSession session, CanvasBitmap mockupImage, Rect mockupRect)
    {
        session.DrawImage(mockupImage, mockupRect);
    }

    public void DrawUserImageOnScreen(CanvasDrawingSession session, CanvasBitmap userImage, Rect screenRect, Rect imageDrawRect,
        (float contrast, float exposure, float tint, float temperature, float saturation, float blur, float pixelScale) effect)
    {
        var combinedEffect = new ContrastEffect
        {
            Name = "ContrastEffect",
            Source = new ExposureEffect
            {
                Name = "ExposureEffect",
                Source = new TemperatureAndTintEffect
                {
                    Name = "TemperatureAndTintEffect",
                    Source = new SaturationEffect
                    {
                        Name = "SaturationEffect",
                        Source = new GaussianBlurEffect
                        {
                            Name = "GaussianBlurEffect",
                            Source = new ScaleEffect
                            {
                                Name = "ScaleDown",
                                Source = new ScaleEffect
                                {
                                    Name = "ScaleUp",
                                    Source = userImage,
                                    Scale = new Vector2(effect.pixelScale, effect.pixelScale), // 放大倍数
                                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                                    BorderMode = EffectBorderMode.Hard
                                },
                                Scale = new Vector2(1f / effect.pixelScale, 1f / effect.pixelScale), // 缩小倍数
                                InterpolationMode = CanvasImageInterpolation.NearestNeighbor,
                                BorderMode = EffectBorderMode.Hard
                            },
                            BlurAmount = effect.blur, // 模糊半径
                            BorderMode = EffectBorderMode.Hard
                        },
                        Saturation = effect.saturation // 0 = 灰度, 1 = 原图, >1 = 增强
                    },
                    Temperature = effect.temperature, // -1 (冷) 到 1 (暖)
                    Tint = effect.tint // -1 (绿) 到 1 (紫)
                },
                Exposure = effect.exposure // -1 (暗) 到 1 (亮)
            },
            Contrast = effect.contrast // 0 (灰度) 到 1 (原图), >1 (增强)
        };

        var destRect = new Rect(0, 0, userImage.SizeInPixels.Width, userImage.SizeInPixels.Height);

        // 创建剪裁区域，确保图片不超出屏幕边界
        using (session.CreateLayer(1.0f, screenRect))
        {
            session.DrawImage(
                combinedEffect,  // 要绘制的效果
                imageDrawRect,   // 输出区域（屏幕上绘制的位置和大小）
                destRect        // 输入区域（原图范围）
                );
        }
    }

    public void DrawScreenBorder(CanvasDrawingSession session, Rect screenRect, bool visible = false)
    {
        if (!visible)
        {
            return;
        }

        // 绘制半透明的红色边框用于调试
        session.DrawRectangle(screenRect, Windows.UI.Color.FromArgb(128, 255, 0, 0), 2.0f);
    }

    public void CleanUp(CanvasDrawingSession session)
    {
        session.Clear(Colors.Transparent);
    }
}
