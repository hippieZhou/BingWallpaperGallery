// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.WinUI.Activation;

public interface IActivationHandler
{
    bool CanHandle(object args);

    Task HandleAsync(object args);
}
