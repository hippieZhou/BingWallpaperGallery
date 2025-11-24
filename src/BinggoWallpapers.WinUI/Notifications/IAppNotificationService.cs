// Copyright (c) hippieZhou. All rights reserved.

using System.Collections.Specialized;
using Microsoft.Windows.AppNotifications;

namespace BinggoWallpapers.WinUI.Notifications;

/// <summary>
/// 应用内消息服务实现
/// </summary>
public interface IAppNotificationService
{
    bool Show(string payload);

    bool Show(AppNotification payload);

    NameValueCollection ParseArguments(string arguments);
}
