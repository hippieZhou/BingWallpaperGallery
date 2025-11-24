// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Services;
using Windows.Globalization;

namespace BinggoWallpapers.WinUI.Selectors.Impl;

public class LanguageSelectorService(ILocalSettingsService localSettingsService) :
    SelectorService(localSettingsService), ILanguageSelectorService
{
    public const string DefaultLanguage = "zh-cn";

    public string Language { get; private set; } = DefaultLanguage;

    protected override string SettingsKey => "AppLanguage";

    public async Task InitializeAsync()
    {
        Language = await ReadFromSettingsAsync(DefaultLanguage);
    }

    public async Task SetLanguageAsync(string language)
    {
        if (string.IsNullOrEmpty(language))
        {
            return;
        }

        Language = language;

        await SetRequestedLanguageAsync();
        await SaveInSettingsAsync(Language);
    }

    public Task SetRequestedLanguageAsync()
    {
        ApplicationLanguages.PrimaryLanguageOverride = Language;
        return Task.CompletedTask;
    }
}
