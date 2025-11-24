// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.WinUI.Notifications;
using BinggoWallpapers.WinUI.Selectors;
using BinggoWallpapers.WinUI.Models;
using BinggoWallpapers.WinUI.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage.Pickers;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace BinggoWallpapers.WinUI.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly ILoggingSelectorService _loggingSelector;
    private readonly IDownloadSelectorService _downloadSelector;
    private readonly ILanguageSelectorService _languageSelector;
    private readonly IThemeSelectorService _themeSelector;
    private readonly IInAppNotificationService _inAppNotificationService;
    private readonly ILogger<SettingsViewModel> _logger;

    public SettingsViewModel(
    ILoggingSelectorService loggingSelector,
    IDownloadSelectorService downloadSelector,
    ILanguageSelectorService languageSelector,
    IThemeSelectorService themeSelector,
    IInAppNotificationService inAppNotificationService,
    ILogger<SettingsViewModel> logger)
    {
        _loggingSelector = loggingSelector;
        _downloadSelector = downloadSelector;
        _languageSelector = languageSelector;
        _themeSelector = themeSelector;
        _inAppNotificationService = inAppNotificationService;
        _logger = logger;

        PicturesPath = _downloadSelector.DownloadPath;
        LogFileSizeInBytes = _loggingSelector.FolderSizeInBytes;

        CurrentLanguage = _languageSelector.Language;
        CurrentTheme = _themeSelector.Theme;
    }

    [ObservableProperty]
    public partial string CurrentLanguage { get; set; }

    [ObservableProperty]
    public partial ElementTheme CurrentTheme { get; set; }

    [ObservableProperty]
    public partial string PicturesPath { get; set; }

    [ObservableProperty]
    public partial long LogFileSizeInBytes { get; set; }

    [RelayCommand]
    private async Task OnSwitchLanguage(string language)
    {
        if (CurrentLanguage != language)
        {
            CurrentLanguage = language;
            await _languageSelector.SetLanguageAsync(language);
        }
    }

    [RelayCommand]
    private async Task OnSwitchTheme(ElementTheme param)
    {
        if (CurrentTheme != param)
        {
            CurrentTheme = param;
            await _themeSelector.SetThemeAsync(param);
        }
    }

    [RelayCommand]
    private async Task OnFetchFromGithub()
    {
        await SynchronizationDialog.Current.ShowAsync();
    }

    [RelayCommand]
    private async Task OnChangeDownloadFolder(string path)
    {
        var folderPicker = new FolderPicker(App.MainWindow.AppWindow.Id)
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            ViewMode = PickerViewMode.List,
        };

        var result = await folderPicker.PickSingleFolderAsync();
        if (result is not null)
        {
            await _downloadSelector.SetDownloadPathAsync(result.Path);
            PicturesPath = _downloadSelector.DownloadPath;
        }
    }

    [RelayCommand]
    private async Task OnResetDownloadFolder()
    {
        await _downloadSelector.SetDownloadPathAsync(AppSettings.Current.DefaultPicturesPath);
        PicturesPath = _downloadSelector.DownloadPath;
    }

    [RelayCommand]
    private void OnLogCleanUp()
    {
        _loggingSelector.CleanUpOldLogs();
        LogFileSizeInBytes = _loggingSelector.FolderSizeInBytes;
    }

    [RelayCommand]
    private async Task OnOpenFolder(string path)
    {
        try
        {
            await Launcher.LaunchFolderPathAsync(path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    [RelayCommand]
    private void OnCloneRepo(string agrs)
    {
        var package = new DataPackage();
        package.SetText(agrs);
        Clipboard.SetContent(package);
        _inAppNotificationService.ShowInfo("命令已复制到剪贴板");
    }

    [RelayCommand]
    private async Task OnBugRequest()
    {
        var subject = "多彩壁纸（BinggoWallpapers）客户反馈";

        var body =
    @$"您好，

我想反馈以下问题：
【请填写问题描述】

设备信息：
- System Version: {AppSettings.OSVersion}
- Application Version: {AppSettings.AppVersion}
- WindowsAppSDK Version:{AppSettings.WinAppSdkRuntimeDetails}

谢谢！";
        var email = "hippiezhou@outlook.com";
        var encodedSubject = Uri.EscapeDataString(subject);
        var encodedBody = Uri.EscapeDataString(body);
        var mailtoUri = new Uri($"mailto:{email}?subject={encodedSubject}&body={encodedBody}");
        await Launcher.LaunchUriAsync(mailtoUri);
    }
}
