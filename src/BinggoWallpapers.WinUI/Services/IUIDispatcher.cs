using Microsoft.UI.Dispatching;

namespace BinggoWallpapers.WinUI.Services;

public interface IUIDispatcher
{
    Task EnqueueAsync(Action action, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal);

    Task EnqueueAsync(Func<Task> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal);

    Task<T> EnqueueAsync<T>(Func<T> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal);

    Task<T> EnqueueAsync<T>(Func<Task<T>> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal);
}
