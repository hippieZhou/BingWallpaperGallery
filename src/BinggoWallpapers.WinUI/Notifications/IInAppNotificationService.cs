// Copyright (c) hippieZhou. All rights reserved.

using CommunityToolkit.WinUI.Behaviors;

namespace BinggoWallpapers.WinUI.Notifications;

/// <summary>
/// 应用内消息服务实现
/// </summary>
public interface IInAppNotificationService
{
    StackedNotificationsBehavior NotificationQueue { get; set; }

    /// <summary>
    /// 发送错误消息
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="title">错误标题</param>
    /// <param name="details">错误详情</param>
    /// <param name="showRetryButton">是否显示重试按钮</param>
    /// <param name="retryAction">重试操作</param>
    void ShowError(string message, string title = "错误", string details = "", bool showRetryButton = false, Action retryAction = null);

    /// <summary>
    /// 发送警告消息
    /// </summary>
    /// <param name="message">警告消息</param>
    /// <param name="title">警告标题</param>
    /// <param name="details">警告详情</param>
    void ShowWarning(string message, string title = "警告", string details = "");

    /// <summary>
    /// 发送信息消息
    /// </summary>
    /// <param name="message">信息消息</param>
    /// <param name="title">信息标题</param>
    /// <param name="details">信息详情</param>
    void ShowInfo(string message, string title = "信息", string details = "");

    /// <summary>
    /// 发送成功消息
    /// </summary>
    /// <param name="message">成功消息</param>
    /// <param name="title">成功标题</param>
    /// <param name="details">成功详情</param>
    void ShowSuccess(string message, string title = "成功", string details = "");
}
