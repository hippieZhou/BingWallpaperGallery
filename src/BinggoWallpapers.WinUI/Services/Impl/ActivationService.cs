// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DataAccess;
using BinggoWallpapers.WinUI.Activation;
using BinggoWallpapers.WinUI.Selectors;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Services.Impl;

public class ActivationService(
    ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
    IEnumerable<IActivationHandler> activationHandlers,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IDownloadSelectorService downloadSelectorService,
    ILanguageSelectorService languageSelectorService,
    IMarketSelectorService regionSelectorService,
    IThemeSelectorService themeSelectorService,
    ILoggingSelectorService loggingSelectorService) : IActivationService
{
    private readonly UIElement _shell = null;

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            App.MainWindow.Content = _shell ?? new Frame();
        }

        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Activate the MainWindow.
        App.MainWindow.Activate();

        // Execute tasks after activation.
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (defaultHandler.CanHandle(activationArgs))
        {
            await defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await ApplicationDbContextInitializer.InitializeAsync(dbContextFactory);

        await downloadSelectorService.InitializeAsync().ConfigureAwait(false);
        await languageSelectorService.InitializeAsync().ConfigureAwait(false);
        await regionSelectorService.InitializeAsync().ConfigureAwait(false);
        await themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await loggingSelectorService.InitializeAsync().ConfigureAwait(false);
    }

    private async Task StartupAsync()
    {
        await downloadSelectorService.SetRequestedDownloadPathAsync();
        await languageSelectorService.SetRequestedLanguageAsync();
        await themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
