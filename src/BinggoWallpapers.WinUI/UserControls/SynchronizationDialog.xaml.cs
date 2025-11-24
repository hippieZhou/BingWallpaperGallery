using BinggoWallpapers.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinggoWallpapers.WinUI.UserControls;

public sealed partial class SynchronizationDialog : ContentDialog
{
    #region Singleton
    static SynchronizationDialog()
    {
        Current = new SynchronizationDialog();
    }

    public static SynchronizationDialog Current { get; }
    #endregion

    public SynchronizationDialogViewModel ViewModel { get; } = new();
    private SynchronizationDialog()
    {
        InitializeComponent();
        XamlRoot = App.MainWindow.Content.XamlRoot;
    }

    private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        args.Cancel = ViewModel.Cancel;
    }

    private void OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        ViewModel.LoadedCommand.Execute(null);
    }
}

public partial class SynchronizationDialogViewModel : ObservableObject
{
    private readonly DispatcherTimer _timer;
    private readonly IMemoryCache _memoryCache;
    private readonly IGitHubStorageService _githubStorage;
    private readonly ILogger<SynchronizationDialogViewModel> _logger;

    [ObservableProperty]
    public partial string Message { get; set; } = "该操作是全量同步，可能会消耗较长时间，请耐心等待...";

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial int Duration { get; set; }

    [ObservableProperty]
    public partial bool Cancel { get; set; } = true;

    public SynchronizationDialogViewModel()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (s, e) => Duration++;

        _memoryCache = App.GetService<IMemoryCache>();
        _githubStorage = App.GetService<IGitHubStorageService>();
        _logger = App.GetService<ILogger<SynchronizationDialogViewModel>>();
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private Task OnLoaded(CancellationToken cancellationToken = default)
    {
        Message = "该操作是全量同步，可能会消耗较长时间，请耐心等待...";
        IsLoading = false;
        Duration = 0;
        return Task.CompletedTask;
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task OnSync(CancellationToken cancellationToken = default)
    {
        Cancel = true;

        if (!_timer.IsEnabled)
        {
            Duration = 0;
            _timer.Start();
        }

        IsLoading = true;

        await _githubStorage.RunAsync(
            onLoading: msg =>
            {
                Message = msg;
                IsLoading = true;
            },
            onEnded: () =>
            {
                Message = "同步完成！您可以关闭此对话框。";
                IsLoading = false;
                _timer.Stop();
                if (_memoryCache is MemoryCache memoryCache)
                {
                    memoryCache.Clear();
                }
            },
            onError: ex =>
            {
                _logger.LogError("获取 GitHub 归档文件失败: {message}", ex.Message);
                Message = "获取 GitHub 归档文件失败，请检查网络连接或稍后重试。";
                IsLoading = false;
                _timer.Stop();
                if (_memoryCache is MemoryCache memoryCache)
                {
                    memoryCache.Clear();
                }
            }, cancellationToken);
    }

    [RelayCommand]
    private void OnClose()
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
        }

        SyncCommand.Cancel();
        Cancel = false;
    }
}
