// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Helpers;
using BinggoWallpapers.WinUI.Options;
using BinggoWallpapers.WinUI.Models;
using Microsoft.Extensions.Options;

namespace BinggoWallpapers.WinUI.Services.Impl;

public class LocalSettingsService : ILocalSettingsService
{
    private const string DefaultApplicationDataFolder = "BinggoWallpapers.WinUI/ApplicationData";
    private const string DefaultLocalSettingsFile = "LocalSettings.json";

    private readonly ILocalStorageService _localStorageService;
    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _localsettingsFile;

    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(ILocalStorageService localStorageService, IOptions<LocalSettingsOptions> options)
    {
        _localStorageService = localStorageService;
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? DefaultApplicationDataFolder);
        _localsettingsFile = _options.LocalSettingsFile ?? DefaultLocalSettingsFile;

        _settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _settings = await Task.Run(() => _localStorageService.ReadAsync<IDictionary<string, object>>(_applicationDataFolder, _localsettingsFile)) ?? new Dictionary<string, object>();

            _isInitialized = true;
        }
    }

    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (AppSettings.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            AppSettings.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
        else
        {
            await InitializeAsync();

            _settings[key] = await Json.StringifyAsync(value);

            await Task.Run(() => _localStorageService.SaveAsync(_applicationDataFolder, _localsettingsFile, _settings));
        }
    }
}
