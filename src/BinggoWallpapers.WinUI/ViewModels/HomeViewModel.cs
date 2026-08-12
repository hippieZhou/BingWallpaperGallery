// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Messages;
using BinggoWallpapers.WinUI.Models;
using BinggoWallpapers.WinUI.Notifications;
using BinggoWallpapers.WinUI.Selectors;
using BinggoWallpapers.WinUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Collections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.BadgeNotifications;

namespace BinggoWallpapers.WinUI.ViewModels;

public partial class HomeViewModel : ObservableRecipient, INavigationAware, IRecipient<RefreshWallpapersCompletedMessage>
{
    private readonly IMarketSelectorService _marketSelectorService;
    private readonly IManagementService _managementService;
    private readonly IInAppNotificationService _inAppNotificationService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<HomeViewModel> _logger;

    [ObservableProperty]
    public partial IncrementalLoadingCollection<WallpaperInfoSource, WallpaperInfoDto> Wallpapers { get; set; }

    [ObservableProperty]
    public partial WallpaperInfoDto Today { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial WallpaperInfoDto SelectedWallpaper { get; set; }

    public HomeViewModel(
        IMarketSelectorService marketSelectorService,
        IManagementService managementService,
        IInAppNotificationService inAppNotificationService,
        IMemoryCache memoryCache,
        ILogger<HomeViewModel> logger)
    {
        _marketSelectorService = marketSelectorService;
        _managementService = managementService;
        _inAppNotificationService = inAppNotificationService;
        _memoryCache = memoryCache;
        _logger = logger;

        IsActive = true;
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task OnLoaded(CancellationToken cancellationToken = default)
    {
        var market = _marketSelectorService.Market;
        Today = await _managementService.GetLatestAsync(market, cancellationToken);
        Wallpapers = _memoryCache.GetOrCreate(market, entry =>
        {
            return new IncrementalLoadingCollection<WallpaperInfoSource, WallpaperInfoDto>(
                source: new WallpaperInfoSource(_marketSelectorService, _managementService),
                itemsPerPage: 10,
                onStartLoading: () => IsLoading = true,
                onEndLoading: () => IsLoading = false,
                onError: ex =>
                {
                    _logger.LogError(ex, $"加载壁纸失败: {ex.Message}");

                    IsLoading = false;

                    _inAppNotificationService.ShowError(
                      message: $"加载壁纸失败: {ex.Message}",
                      title: "加载失败",
                      details: ex.ToString(),
                      showRetryButton: true,
                      retryAction: () => LoadedCommand.Execute(null));
                });
        });
    }

    [RelayCommand]
    private void OnWallpaperSelected(WallpaperInfoDto wallpaper)
    {
        SelectedWallpaper = wallpaper;

        // 导航到详情页
        var navigationService = App.GetService<INavigationService>();
        navigationService.NavigateTo<DetailViewModel>(wallpaper);
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true, AllowConcurrentExecutions = false)]
    private async Task Refresh(CancellationToken cancellationToken)
    {
        IsLoading = true;
        try
        {
            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.Activity);

            await Task.Run(async () =>
            {
                await _managementService.RunCollectionAsync(cancellationToken);
                if (_memoryCache is MemoryCache memoryCache)
                {
                    memoryCache.Clear();
                }
            }, cancellationToken);
            _inAppNotificationService.ShowSuccess("所有壁纸信息收集完成！");
        }
        catch (Exception ex)
        {
            _inAppNotificationService.ShowError(ex.Message);
        }
        finally
        {
            BadgeNotificationManager.Current.ClearBadge();
            IsLoading = false;
        }
    }

    public void Receive(RefreshWallpapersCompletedMessage message)
    {
        LoadedCommand.Execute(CancellationToken.None);
    }
}
