// Copyright (c) hippieZhou. All rights reserved.

using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Retry;

namespace BinggoWallpapers.Core.Http.Configuration;

/// <summary>
/// HTTP 弹性配置
/// </summary>
public static class ResilienceConfiguration
{
    /// <summary>
    /// 创建标准的 HTTP 重试策略
    /// </summary>
    public static void ConfigureStandardResilience(HttpStandardResilienceOptions options)
    {
        // 重试策略配置
        options.Retry = new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromSeconds(15),
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .Handle<TaskCanceledException>()
                .HandleResult(response => !response.IsSuccessStatusCode &&
                             (response.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                              response.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                              response.StatusCode == System.Net.HttpStatusCode.TooManyRequests))
        };

        // 超时策略配置
        options.TotalRequestTimeout = new HttpTimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        // 熔断器配置
        options.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5, // 50% 失败率触发熔断
            MinimumThroughput = 10, // 最少10个请求后才考虑熔断
            SamplingDuration = TimeSpan.FromSeconds(30), // 采样时间窗口
            BreakDuration = TimeSpan.FromSeconds(60), // 熔断持续时间
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                .Handle<HttpRequestException>()
                .Handle<TaskCanceledException>()
                .HandleResult(response => !response.IsSuccessStatusCode)
        };
    }

    /// <summary>
    /// 创建数据库操作的重试策略
    /// </summary>
    public static ResiliencePipeline CreateDatabaseRetryPipeline()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(500),
                MaxDelay = TimeSpan.FromSeconds(5),
                ShouldHandle = new PredicateBuilder()
                    .Handle<Microsoft.Data.Sqlite.SqliteException>()
                    .Handle<Microsoft.EntityFrameworkCore.DbUpdateException>()
                    .Handle<TimeoutException>()
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .Build();
    }

    /// <summary>
    /// 创建文件操作的重试策略
    /// </summary>
    public static ResiliencePipeline CreateFileOperationRetryPipeline()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Linear,
                Delay = TimeSpan.FromMilliseconds(100),
                MaxDelay = TimeSpan.FromSeconds(2),
                ShouldHandle = new PredicateBuilder()
                    .Handle<IOException>()
                    .Handle<UnauthorizedAccessException>()
                    .Handle<DirectoryNotFoundException>()
            })
            .Build();
    }
}
