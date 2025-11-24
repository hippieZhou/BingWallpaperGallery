// Copyright (c) hippieZhou. All rights reserved.

using System.Numerics;

namespace BinggoWallpapers.WinUI.Models;

public class DeviceConfiguration
{
    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// Mockup图片URI（本地或网络地址）
    /// </summary>
    public string MockupImageUri { get; set; } = string.Empty;

    /// <summary>
    /// Mockup图片的实际像素尺寸
    /// </summary>
    public Vector2 MockupImageSize { get; set; }

    /// <summary>
    /// 设备屏幕的实际分辨率
    /// </summary>
    public Vector2 ScreenResolution { get; set; }

    /// <summary>
    /// 设备屏幕的纵横比
    /// </summary>
    public double ScreenAspectRatio { get; set; }

    /// <summary>
    /// 屏幕区域在Mockup图片中的左上角相对坐标 (0-1)
    /// </summary>
    public Vector2 ScreenTopLeft { get; set; }

    /// <summary>
    /// 屏幕区域在Mockup图片中的右下角相对坐标 (0-1)
    /// </summary>
    public Vector2 ScreenBottomRight { get; set; }

    /// <summary>
    /// 创建Surface Book 15英寸配置
    /// </summary>
    public static DeviceConfiguration CreateSurfaceBook15()
    {
        return new DeviceConfiguration
        {
            DeviceName = "Microsoft Surface Book",
            MockupImageUri = "ms-appx:///Assets/Mockup/microsoft-surface-book.png",
            MockupImageSize = new Vector2(4097, 2396),
            ScreenResolution = new Vector2(3240, 2160),
            ScreenAspectRatio = 3.0 / 2.0,
            ScreenTopLeft = new Vector2(548f / 4097f, 158f / 2396f),
            ScreenBottomRight = new Vector2(3550f / 4097f, 2160f / 2396f)
        };
    }
}
