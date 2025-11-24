namespace BinggoWallpapers.WinUI.Selectors;

public interface ILoggingSelectorService
{
    long FolderSizeInBytes { get; }
    Task InitializeAsync();
    void CleanUpOldLogs();
}
