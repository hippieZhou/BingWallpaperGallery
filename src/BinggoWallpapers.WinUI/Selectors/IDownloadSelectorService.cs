namespace BinggoWallpapers.WinUI.Selectors;

public interface IDownloadSelectorService
{
    string DownloadPath { get; }

    Task InitializeAsync(CancellationToken none = default);
    Task SetRequestedDownloadPathAsync();

    Task SetDownloadPathAsync(string picturesPath);
}
