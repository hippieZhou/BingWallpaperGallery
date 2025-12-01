using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace BinggoWallpapers.WinUI.Services.Impl;

public sealed class UIDispatcher(DispatcherQueue dispatcherQueue) : IUIDispatcher
{
    public Task EnqueueAsync(Action action, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal)
    {
        return dispatcherQueue.EnqueueAsync(action, priority);
    }

    public Task EnqueueAsync(Func<Task> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal)
    {
        return dispatcherQueue.EnqueueAsync(func, priority);
    }

    public Task<T> EnqueueAsync<T>(Func<T> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal)
    {
        return dispatcherQueue.EnqueueAsync(func, priority);
    }

    public Task<T> EnqueueAsync<T>(Func<Task<T>> func, DispatcherQueuePriority priority = DispatcherQueuePriority.Normal)
    {
        return dispatcherQueue.EnqueueAsync(func, priority);
    }
}

