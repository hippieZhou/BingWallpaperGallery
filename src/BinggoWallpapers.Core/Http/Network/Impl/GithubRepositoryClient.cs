using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Models;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Http.Network.Impl;

public class GithubRepositoryClient(
    HttpClient httpClient,
    ILogger<GithubRepositoryClient> logger) : IGithubRepositoryClient
{
    public async Task<IEnumerable<ArchiveItem>> GetArchiveAsync(string path, CancellationToken cancellationToken = default)
    {
        // GitHub API 地址
        var apiUrl = BuildApiUrl(
            owner: "hippieZhou",
            repo: "BinggoWallpapers",
            path: path,
            branch: "main");

        try
        {
            // 获取目录内容
            var response = await httpClient.GetAsync(apiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var items = await Json.ToObjectAsync<IEnumerable<ArchiveItem>>(json);
            return items;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "从 GitHub 仓库获取归档信息失败: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<WallpaperInfoStorage> GetArchiveFileAsync(string downloadUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync(downloadUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return await Json.ToObjectAsync<WallpaperInfoStorage>(json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "从 GitHub 仓库获取归档文件失败: {Message}", ex.Message);
            throw;
        }
    }

    private static string BuildApiUrl(string owner, string repo, string path, string branch) => $"https://api.github.com/repos/{owner}/{repo}/contents/{path}?ref={branch}";
}
