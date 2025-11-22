// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core;
using BinggoWallpapers.WinUI.Activation;
using BinggoWallpapers.WinUI.Extensions;
using BinggoWallpapers.WinUI.Notifications;
using BinggoWallpapers.WinUI.Notifications.Impl;
using BinggoWallpapers.WinUI.Options;
using BinggoWallpapers.WinUI.Selectors;
using BinggoWallpapers.WinUI.Selectors.Impl;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.Services.Impl;
using BinggoWallpapers.WinUI.Models;
using BinggoWallpapers.WinUI.ViewModels;
using BinggoWallpapers.WinUI.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Serilog;

namespace BinggoWallpapers.WinUI;

internal class Bootstrapper
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private readonly IHost _host;

    public Bootstrapper()
    {
        _host = Host
            .CreateDefaultBuilder()
            .UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
             {
                 // 初始化 Serilog
                 var logBaseDir = AppSettings.Current.DefaulttLocalLogFolder;
                 var logger = context.Configuration.ConfigureLogger(logBaseDir);

                 services.AddLogging(x =>
                 {
                     x.ClearProviders();
                     x.AddSerilog(logger);
                 });

                 // Default Activation Handler
                 services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                 // Other Activation Handlers
                 services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

                 #region Selectors
                 services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                 services.AddSingleton<IMarketSelectorService, MarketSelectorService>();
                 services.AddSingleton<ILanguageSelectorService, LanguageSelectorService>();
                 services.AddSingleton<IDownloadSelectorService, DownloadSelectorService>();
                 services.AddSingleton<ILoggingSelectorService, LoggingSelectorService>();
                 #endregion

                 #region Notifications
                 services.AddActivatedSingleton<IAppNotificationService, AppNotificationService>();
                 services.AddActivatedSingleton<IInAppNotificationService, InAppNotificationService>();
                 services.AddSingleton<IMessenger, WeakReferenceMessenger>();
                 #endregion

                 #region Services
                 services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                 services.AddSingleton<IActivationService, ActivationService>();
                 services.AddSingleton<IPageService, PageService>();
                 services.AddSingleton<INavigationService, NavigationService>();
                 services.AddSingleton<INavigationViewService, NavigationViewService>();
                 services.AddSingleton<IImageRenderService, ImageRenderService>();
                 services.AddSingleton<IImageExportService, ImageExportService>();
                 #endregion

                 #region Views and ViewModels
                 services.AddTransient<SettingsViewModel>();
                 services.AddTransient<SettingsPage>();
                 services.AddTransient<DownloadViewModel>();
                 services.AddTransient<DownloadPage>();
                 services.AddTransient<HomeViewModel>();
                 services.AddTransient<HomePage>();
                 services.AddTransient<DetailViewModel>();
                 services.AddTransient<DetailPage>();
                 services.AddTransient<ShellViewModel>();
                 #endregion

                 // Configuration
                 services.AddOptionsWithValidateOnStart<LocalSettingsOptions>()
                  .BindConfiguration(nameof(LocalSettingsOptions))
                  .ValidateDataAnnotations();
                 services.AddOptionsWithValidateOnStart<LoggingOptions>()
                 .BindConfiguration(nameof(LoggingOptions))
                 .ValidateDataAnnotations();

                 // Core Services
                 services.AddCorelayer(AppSettings.Current.LocalFolder);
             })
#if DEBUG
            .UseEnvironment(Environments.Development)
#else
            .UseEnvironment(Environments.Production)
#endif
            .Build();
    }

    public T GetService<T>() where T : class
    {
        if (_host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    internal async Task StartAsync(LaunchActivatedEventArgs args)
    {
        _host.Start();

        var activation = App.GetService<IActivationService>();
        await activation.ActivateAsync(args);

        //var notification = GetService<IAppNotificationService>();
        //notification.Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        App.GetService<ILogger<App>>().LogInformation("The app has been launched successfully.");
    }

    internal void HandleException(Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        GetService<ILogger<App>>().LogError(e.Exception, e.Message);
    }
}
