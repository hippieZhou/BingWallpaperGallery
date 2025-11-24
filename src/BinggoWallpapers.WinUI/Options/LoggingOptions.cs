// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.WinUI.Options;

/// <summary>
/// 日志配置选项
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// 日志保留天数，默认 30 天
    /// </summary>
    public int RetainedDays { get; set; } = 30;

    /// <summary>
    /// 单个日志文件大小限制（字节），默认 10MB
    /// </summary>
    public long FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// 保留的日志文件数量限制，默认 5 个
    /// </summary>
    public int RetainedFileCountLimit { get; set; } = 5;

    /// <summary>
    /// 最小日志级别，默认 Verbose
    /// </summary>
    public string MinimumLevel { get; set; } = "Verbose";

    /// <summary>
    /// 是否启用结构化日志（JSON格式），默认 true
    /// </summary>
    public bool EnableStructuredLogging { get; set; } = true;

    /// <summary>
    /// 是否启用调试输出，默认 true
    /// </summary>
    public bool EnableDebugOutput { get; set; } = true;
}
