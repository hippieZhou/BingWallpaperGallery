using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.Models;

namespace BinggoWallpapers.WinUI.Selectors.Impl;

public class DownloadSelectorService(
    ILocalSettingsService localSettingsService,
    IDownloadService downloadService) :
    SelectorService(localSettingsService), IDownloadSelectorService
{
    public string DownloadPath { get; private set; } = AppSettings.Current.DefaultPicturesPath;

    protected override string SettingsKey => "AppDownload";

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        DownloadPath = await ReadFromSettingsAsync(AppSettings.Current.DefaultPicturesPath);
    }

    public async Task SetRequestedDownloadPathAsync()
    {
        await downloadService.SetRequestedDownloadPathAsync(DownloadPath);
    }

    public async Task SetDownloadPathAsync(string picturesPath)
    {
        DownloadPath = picturesPath;

        await SetRequestedDownloadPathAsync();
        await SaveInSettingsAsync(DownloadPath);
    }
}
