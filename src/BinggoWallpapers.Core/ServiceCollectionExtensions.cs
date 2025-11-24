// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DataAccess;
using BinggoWallpapers.Core.DataAccess.Interceptors;
using BinggoWallpapers.Core.DataAccess.Repositories;
using BinggoWallpapers.Core.DataAccess.Repositories.Impl;
using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Network;
using BinggoWallpapers.Core.Http.Network.Impl;
using BinggoWallpapers.Core.Http.Options;
using BinggoWallpapers.Core.Http.Services;
using BinggoWallpapers.Core.Http.Services.Impl;
using BinggoWallpapers.Core.Services;
using BinggoWallpapers.Core.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BinggoWallpapers.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorelayer(
        this IServiceCollection services,
        string appDataPath)
    {
        services.AddSingleton<IValidateOptions<CollectionOptions>, CollectionOptionsValidator>();
        services
            .AddOptionsWithValidateOnStart<CollectionOptions>()
            .BindConfiguration(nameof(CollectionOptions))
            .ValidateDataAnnotations();

        services.AddBingWallpaperCollector();
        services.AddInfrastructure(appDataPath);

        services.AddMemoryCache();

        #region Services
        services.AddSingleton<IManagementService, ManagementService>();
        services.AddSingleton<IGitHubStorageService, GitHubStorageService>();
        services.AddSingleton<ILocalStorageService, LocalStorageService>();
        services.AddSingleton<IDownloadService, DownloadService>();
        #endregion

        return services;
    }

    private static void AddBingWallpaperCollector(this IServiceCollection services)
    {
        // 配置HttpClient with 弹性策略
        services.AddHttpClient<IBingWallpaperClient, BingWallpaperClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(HTTPConstants.HttpTimeoutSeconds);
            client.DefaultRequestHeaders.Add("User-Agent", HTTPConstants.HttpHeaders.UserAgent);
        }).AddStandardResilienceHandler(ResilienceConfiguration.ConfigureStandardResilience);

        services.AddHttpClient<IImageDownloadClient, ImageDownloadClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(HTTPConstants.HttpTimeoutSeconds);
            client.DefaultRequestHeaders.Add("User-Agent", HTTPConstants.HttpHeaders.UserAgent);
        }).AddStandardResilienceHandler(ResilienceConfiguration.ConfigureStandardResilience);

        services.AddHttpClient<IGithubRepositoryClient, GithubRepositoryClient>(client =>
        {
            client.BaseAddress = new Uri(HTTPConstants.GithubRepositoryBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(HTTPConstants.HttpTimeoutSeconds);
            client.DefaultRequestHeaders.Add("User-Agent", HTTPConstants.HttpHeaders.UserAgent);
        }).AddStandardResilienceHandler(ResilienceConfiguration.ConfigureStandardResilience);

        // 注册服务
        services.AddScoped<IBingWallpaperService, BingWallpaperService>();
        services.AddScoped<IImageDownloadService, ImageDownloadService>();
        services.AddScoped<IGithubRepositoryService, GithubRepositoryService>();
    }

    private static void AddInfrastructure(this IServiceCollection services, string appDataPath)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<IDbConnectionInterceptor, SqliteJournalModeSettingInterceptor>();

        services.AddPooledDbContextFactory<ApplicationDbContext>((sp, options) =>
        {
            using var scope = sp.CreateScope();
            options.AddInterceptors(scope.ServiceProvider.GetServices<ISaveChangesInterceptor>());
            options.AddInterceptors(scope.ServiceProvider.GetServices<IDbConnectionInterceptor>());
            var dbFile = Path.Combine(appDataPath, "database.db");

            var migrationsAssemblyName = typeof(ApplicationDbContext).Assembly.GetName().Name;
            options.UseSqlite($"Data Source={dbFile}", db => db.MigrationsAssembly(migrationsAssemblyName));
        });

        #region Repositories
        services.AddScoped<IWallpaperRepository, WallpaperRepository>();
        #endregion
    }
}
