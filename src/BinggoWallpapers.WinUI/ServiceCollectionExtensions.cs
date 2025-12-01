using System.Runtime.InteropServices;
using BinggoWallpapers.WinUI.Activation;
using BinggoWallpapers.WinUI.Models;
using BinggoWallpapers.WinUI.Notifications;
using BinggoWallpapers.WinUI.Notifications.Impl;
using BinggoWallpapers.WinUI.Options;
using BinggoWallpapers.WinUI.Selectors;
using BinggoWallpapers.WinUI.Selectors.Impl;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.Services.Impl;
using BinggoWallpapers.WinUI.ViewModels;
using BinggoWallpapers.WinUI.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace BinggoWallpapers.WinUI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration, string appSettingsFileName = "appsettings.json")
    {
        var logBaseDir = AppInfo.AppLogsPath;
        services.AddSerilog(loggerConfig =>
        {
            // 从配置文件读取日志选项
            var options = configuration.GetSection(nameof(LoggingOptions)).Get<LoggingOptions>();

            // 解析最小日志级别
            var minimumLevel = Enum.TryParse<LogEventLevel>(options.MinimumLevel, true, out var level)
                ? level
                : LogEventLevel.Verbose;

            loggerConfig
           .Enrich.WithExceptionDetails()
           .Enrich.FromLogContext()
           .Enrich.WithProperty("AppTitle", AppInfo.AppTitle)
           .Enrich.WithProperty("AppVersion", AppInfo.AppVersion)
           .Enrich.WithProperty("OSVersion", AppInfo.OSVersion)
           .Enrich.WithProperty("OSArchitecture", RuntimeInformation.OSArchitecture)
           .Enrich.WithProperty("OSDescription", RuntimeInformation.OSDescription)
           .Enrich.WithProperty("ProcessArchitecture", RuntimeInformation.ProcessArchitecture)
           .Enrich.WithProperty("RuntimeIdentifier", RuntimeInformation.RuntimeIdentifier)
           .Enrich.WithProperty("FrameworkDescription", RuntimeInformation.FrameworkDescription)
           .MinimumLevel.Is(minimumLevel)
           // 过滤第三方库的日志
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
           .MinimumLevel.Override("System", LogEventLevel.Warning)
           .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
           .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);

            // 调试输出（可选）
            if (options.EnableDebugOutput)
            {
                loggerConfig.WriteTo.Debug(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{AppTitle}] {Message:lj}{NewLine}{Exception}",
                    restrictedToMinimumLevel: minimumLevel
                );
            }

            // 主日志文件（包含所有级别）
            loggerConfig.WriteTo.Map(
                keyPropertyName: "UtcDateTime",
                defaultKey: DateTime.UtcNow.ToString("yyyyMMdd"),
                configure: (date, writeTo) =>
                {
                    var dir = Path.Combine(logBaseDir, date);
                    Directory.CreateDirectory(dir);

                    writeTo.File(
                        Path.Combine(dir, "app.log"),
                        restrictedToMinimumLevel: minimumLevel,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{AppTitle}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Infinite,
                        fileSizeLimitBytes: options.FileSizeLimitBytes,
                        retainedFileCountLimit: options.RetainedFileCountLimit,
                        rollOnFileSizeLimit: true
                    );
                });

            // 错误日志文件（仅 Error 和 Fatal）
            loggerConfig.WriteTo.Map(
                keyPropertyName: "UtcDateTime",
                defaultKey: DateTime.UtcNow.ToString("yyyyMMdd"),
                configure: (date, writeTo) =>
                {
                    var dir = Path.Combine(logBaseDir, date);
                    Directory.CreateDirectory(dir);

                    writeTo.File(
                        Path.Combine(dir, "error.log"),
                        restrictedToMinimumLevel: LogEventLevel.Error,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{AppTitle}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                        rollingInterval: RollingInterval.Infinite,
                        fileSizeLimitBytes: options.FileSizeLimitBytes,
                        retainedFileCountLimit: options.RetainedFileCountLimit,
                        rollOnFileSizeLimit: true
                    );
                });

            // 结构化日志（JSON格式，可选）
            if (options.EnableStructuredLogging)
            {
                loggerConfig.WriteTo.Map(
                    keyPropertyName: "UtcDateTime",
                    defaultKey: DateTime.UtcNow.ToString("yyyyMMdd"),
                    configure: (date, writeTo) =>
                    {
                        var dir = Path.Combine(logBaseDir, date);
                        Directory.CreateDirectory(dir);

                        writeTo.File(
                            new CompactJsonFormatter(),
                            Path.Combine(dir, "structured.json"),
                            restrictedToMinimumLevel: LogEventLevel.Information,
                            rollingInterval: RollingInterval.Infinite,
                            fileSizeLimitBytes: options.FileSizeLimitBytes,
                            retainedFileCountLimit: options.RetainedFileCountLimit,
                            rollOnFileSizeLimit: true
                        );
                    });
            }
        });

        return services;
    }

    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddSingleton<IMarketSelectorService, MarketSelectorService>();
        services.AddSingleton<ILanguageSelectorService, LanguageSelectorService>();
        services.AddSingleton<IDownloadSelectorService, DownloadSelectorService>();
        services.AddSingleton<ILoggingSelectorService, LoggingSelectorService>();
        return services;
    }

    public static IServiceCollection AddMvvm(this IServiceCollection services)
    {
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<DownloadViewModel>();
        services.AddTransient<DownloadPage>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<HomePage>();
        services.AddTransient<DetailViewModel>();
        services.AddTransient<DetailPage>();
        services.AddTransient<ShellViewModel>();
        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<LocalSettingsOptions>()
            .BindConfiguration(nameof(LocalSettingsOptions))
            .ValidateDataAnnotations();
        services.AddOptionsWithValidateOnStart<LoggingOptions>()
            .BindConfiguration(nameof(LoggingOptions))
            .ValidateDataAnnotations();
        return services;
    }

    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue)
    {
        services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
        services.AddSingleton<IActivationService, ActivationService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<INavigationViewService, NavigationViewService>();
        services.AddSingleton<IImageRenderService, ImageRenderService>();
        services.AddSingleton<IImageExportService, ImageExportService>();

        // Dispatcher Queue
        services.AddSingleton(dispatcherQueue);
        services.AddSingleton<IUIDispatcher, UIDispatcher>();

        // Notification
        services.AddActivatedSingleton<IAppNotificationService, AppNotificationService>();
        services.AddActivatedSingleton<IInAppNotificationService, InAppNotificationService>();
        services.AddSingleton<IMessenger, WeakReferenceMessenger>();

        // Default Activation Handler
        services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

        // Other Activation Handlers
        services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

        return services;
    }
}
