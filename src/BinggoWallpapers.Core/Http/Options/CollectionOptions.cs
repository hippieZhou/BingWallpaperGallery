using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Enums;
using Microsoft.Extensions.Options;

namespace BinggoWallpapers.Core.Http.Options;
public class CollectionOptions
{
    public MarketCode MarketCode { get; set; } = MarketCode.China;
    public ResolutionCode ResolutionCode { get; set; } = ResolutionCode.FullHD;
    public bool CollectAllCountries { get; set; } = true;
    public int CollectDays { get; set; } = HTTPConstants.MaxHistoryDays;
    public bool PrettyJsonFormat { get; set; } = true;
    public int MaxConcurrentRequests { get; set; } = HTTPConstants.DefaultConcurrentRequests;
    public int MaxConcurrentDownloads { get; set; } = HTTPConstants.DefaultConcurrentDownloads;
}

public class CollectionOptionsValidator : IValidateOptions<CollectionOptions>
{
    public ValidateOptionsResult Validate(string name, CollectionOptions options)
    {
        // 设置收集天数
        if (options.CollectDays is < 1 or > HTTPConstants.MaxHistoryDays)
        {
            return ValidateOptionsResult.Fail(
                $"CollectDays must be between 1 and {HTTPConstants.MaxHistoryDays}.");
        }

        // 设置并发请求数
        if (options.MaxConcurrentRequests is < 1 or > HTTPConstants.MaxConcurrentRequests)
        {
            return ValidateOptionsResult.Fail(
                $"MaxConcurrentRequests must be between 1 and {HTTPConstants.MaxConcurrentRequests}.");
        }

        // 设置并发下载数
        if (options.MaxConcurrentDownloads is < 1 or > HTTPConstants.MaxConcurrentDownloads)
        {
            return ValidateOptionsResult.Fail(
                $"MaxConcurrentDownloads must be between 1 and {HTTPConstants.MaxConcurrentDownloads}.");
        }

        return ValidateOptionsResult.Success;
    }
}
