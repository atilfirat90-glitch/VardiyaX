using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ShiftCraft.Mobile.Services;

public class OfflineModeService : IOfflineModeService
{
    private readonly IConnectivityService _connectivityService;
    
    public event EventHandler<bool>? OfflineModeChanged;

    public OfflineModeService(IConnectivityService connectivityService)
    {
        _connectivityService = connectivityService;
        _connectivityService.ConnectivityChanged += OnConnectivityChanged;
    }

    public bool IsOfflineMode => !_connectivityService.IsOnline;

    public async Task<T> ExecuteWithOfflineSupportAsync<T>(
        Func<Task<T>> onlineAction,
        Func<Task<T>> offlineAction)
    {
        if (_connectivityService.IsOnline)
        {
            try
            {
                return await onlineAction();
            }
            catch (HttpRequestException)
            {
                // Network error, fallback to offline
                return await offlineAction();
            }
        }
        
        return await offlineAction();
    }

    public async Task ExecuteWriteOperationAsync(Func<Task> action)
    {
        if (!_connectivityService.IsOnline)
        {
            ShowOfflineToast();
            throw new OfflineException();
        }

        await action();
    }

    public void ShowOfflineToast()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var toast = Toast.Make(
                "Çevrimdışı - Bu işlem internet bağlantısı gerektirir",
                ToastDuration.Short,
                14);
            await toast.Show();
        });
    }

    private void OnConnectivityChanged(object? sender, bool isOnline)
    {
        OfflineModeChanged?.Invoke(this, !isOnline);
        
        if (isOnline)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var toast = Toast.Make("Çevrimiçi - Bağlantı sağlandı", ToastDuration.Short, 14);
                await toast.Show();
            });
        }
    }
}

public class OfflineException : Exception
{
    public OfflineException() 
        : base("Çevrimdışı - Bu işlem internet bağlantısı gerektirir") { }
}
