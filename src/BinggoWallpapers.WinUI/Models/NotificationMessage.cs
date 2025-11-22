// Copyright (c) hippieZhou. All rights reserved.

using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Models;

/// <summary>
/// 错误消息模型
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="message">错误消息</param>
/// <param name="title">错误标题</param>
/// <param name="details">错误详情</param>
/// <param name="severity">错误严重程度</param>
/// <param name="showRetryButton">是否显示重试按钮</param>
/// <param name="retryAction">重试命令</param>
public class NotificationMessage(
    string message,
    string title = "错误",
    string details = "",
    InfoBarSeverity severity = InfoBarSeverity.Error,
    bool showRetryButton = false,
    Action retryAction = null) : ValueChangedMessage<string>(message)
{
    /// <summary>
    /// 错误标题
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// 错误详情
    /// </summary>
    public string Details { get; } = details;

    /// <summary>
    /// 错误严重程度
    /// </summary>
    public InfoBarSeverity Severity { get; } = severity;

    /// <summary>
    /// 是否显示重试按钮
    /// </summary>
    public bool ShowRetryButton { get; } = showRetryButton;

    /// <summary>
    /// 重试命令
    /// </summary>
    public Action RetryAction { get; } = retryAction;
}
