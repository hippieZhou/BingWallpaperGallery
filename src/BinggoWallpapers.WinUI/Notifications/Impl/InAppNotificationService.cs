// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Notifications.Impl;

/// <summary>
/// 应用内消息服务实现
/// </summary>
public class InAppNotificationService : IInAppNotificationService
{
    private readonly IMessenger _messenger;
    private readonly ILogger<InAppNotificationService> _logger;

    public InAppNotificationService(
        IMessenger messenger,
        ILogger<InAppNotificationService> logger)
    {
        _messenger = messenger;
        _logger = logger;
        Initialize();
    }

    ~InAppNotificationService()
    {
        UnInitialize();
    }

    public StackedNotificationsBehavior NotificationQueue { get; set; }

    private void Initialize()
    {
        _messenger.Register<InAppNotificationService, NotificationMessage>(this,
            (r, m) =>
            {
                var notification = new Notification
                {
                    Duration = TimeSpan.FromSeconds(4),
                    Title = m.Title,
                    Message = m.Value,
                    Severity = m.Severity
                };

                if (m.ShowRetryButton)
                {
                    notification.ActionButton = new Button
                    {
                        Content = "重试",
                        Command = new RelayCommand(() =>
                        {
                            m.RetryAction?.Invoke();
                        })
                    };
                }

                r.NotificationQueue.Show(notification);
            });
    }

    public void UnInitialize()
    {
        _messenger.Unregister<NotificationMessage>(this);
    }

    /// <summary>
    /// 发送错误消息
    /// </summary>
    public void ShowError(string message, string title = "错误", string details = "", bool showRetryButton = false, Action retryAction = null)
    {
        try
        {
            var errorMessage = new NotificationMessage(
                message: message,
                title: title,
                details: details,
                severity: InfoBarSeverity.Error,
                showRetryButton: showRetryButton,
                retryAction: retryAction
            );

            _messenger.Send(errorMessage);
            _logger.LogError("发送错误消息: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送错误消息失败: {Message}", message);
        }
    }

    /// <summary>
    /// 发送警告消息
    /// </summary>
    public void ShowWarning(string message, string title = "警告", string details = "")
    {
        try
        {
            var errorMessage = new NotificationMessage(
                message: message,
                title: title,
                details: details,
                severity: InfoBarSeverity.Warning
            );

            _messenger.Send(errorMessage);
            _logger.LogWarning("发送警告消息: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送警告消息失败: {Message}", message);
        }
    }

    /// <summary>
    /// 发送信息消息
    /// </summary>
    public void ShowInfo(string message, string title = "信息", string details = "")
    {
        try
        {
            var errorMessage = new NotificationMessage(
                message: message,
                title: title,
                details: details,
                severity: InfoBarSeverity.Informational
            );

            _messenger.Send(errorMessage);
            _logger.LogInformation("发送信息消息: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送信息消息失败: {Message}", message);
        }
    }

    /// <summary>
    /// 发送成功消息
    /// </summary>
    public void ShowSuccess(string message, string title = "成功", string details = "")
    {
        try
        {
            var errorMessage = new NotificationMessage(
                message: message,
                title: title,
                details: details,
                severity: InfoBarSeverity.Success
            );

            _messenger.Send(errorMessage);
            _logger.LogInformation("发送成功消息: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送成功消息失败: {Message}", message);
        }
    }
}
