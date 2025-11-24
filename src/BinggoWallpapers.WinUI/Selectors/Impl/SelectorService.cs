using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.WinUI.Services;

namespace BinggoWallpapers.WinUI.Selectors.Impl;

public abstract class SelectorService(ILocalSettingsService localSettingsService)
{
    protected abstract string SettingsKey { get; }

    protected async Task<T> ReadFromSettingsAsync<T>(T defaultSetting)
    {
        var rawSetting = await localSettingsService.ReadSettingAsync<string>(SettingsKey);
        return !string.IsNullOrWhiteSpace(rawSetting) ?
                rawSetting is T t ?
                    t :
                    await Json.ToObjectAsync<T>(rawSetting) :
            defaultSetting;
    }

    protected async Task SaveInSettingsAsync<T>(T setting)
    {
        var rawSetting = setting is string ?
           setting!.ToString() :
           await Json.StringifyAsync(setting);

        await localSettingsService.SaveSettingAsync(SettingsKey, rawSetting);
    }
}
