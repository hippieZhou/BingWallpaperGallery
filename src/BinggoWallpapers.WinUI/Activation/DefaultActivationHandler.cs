// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.ViewModels;
using Microsoft.UI.Xaml;

namespace BinggoWallpapers.WinUI.Activation;

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
