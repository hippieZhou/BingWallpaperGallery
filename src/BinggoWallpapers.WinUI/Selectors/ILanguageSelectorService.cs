// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.WinUI.Selectors;

public interface ILanguageSelectorService
{
    string Language
    {
        get;
    }

    Task InitializeAsync();

    Task SetLanguageAsync(string language);
    Task SetRequestedLanguageAsync();
}
