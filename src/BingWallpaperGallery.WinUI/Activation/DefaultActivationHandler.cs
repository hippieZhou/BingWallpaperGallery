// Copyright (c) hippieZhou. All rights reserved.

using BingWallpaperGallery.WinUI.Services;
using BingWallpaperGallery.WinUI.ViewModels;
using Microsoft.UI.Xaml;

namespace BingWallpaperGallery.WinUI.Activation;

public class DefaultActivationHandler(INavigationService navigationService) : ActivationHandler<LaunchActivatedEventArgs>
{
    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return navigationService.Frame?.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        navigationService.NavigateTo(typeof(HomeViewModel).FullName!, args.Arguments);

        await Task.CompletedTask;
    }
}
