// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.WinUI.Services;

public interface INavigationAware
{
    void OnNavigatedTo(object parameter);

    void OnNavigatedFrom();
}
