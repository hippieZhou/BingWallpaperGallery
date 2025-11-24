// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.WinUI.Services;

public interface ILocalSettingsService
{
    Task<T?> ReadSettingAsync<T>(string key);

    Task SaveSettingAsync<T>(string key, T value);
}
