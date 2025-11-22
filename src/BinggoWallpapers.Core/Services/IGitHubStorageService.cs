namespace BinggoWallpapers.Core.Services;
public interface IGitHubStorageService
{
    Task RunAsync(
        Action<string> onLoading = null,
        Action onEnded = null,
        Action<Exception> onError = null,
        CancellationToken cancellationToken = default);
}
