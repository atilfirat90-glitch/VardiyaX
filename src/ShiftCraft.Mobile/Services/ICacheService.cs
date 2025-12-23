using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public interface ICacheService
{
    Task CacheScheduleAsync(WeeklySchedule schedule);
    Task<WeeklySchedule?> GetCachedScheduleAsync(DateTime weekStart);
    Task<List<WeeklySchedule>> GetAllCachedSchedulesAsync();
    Task ClearOldCacheAsync(int keepWeeks = 4);
    Task<DateTime?> GetLastSyncTimeAsync();
    Task SetLastSyncTimeAsync(DateTime syncTime);
    Task<bool> HasCachedDataAsync();
    Task ClearAllCacheAsync();
}

public interface IConnectivityService
{
    bool IsOnline { get; }
    event EventHandler<bool> ConnectivityChanged;
    Task<bool> CheckConnectivityAsync();
}

public interface IOfflineModeService
{
    bool IsOfflineMode { get; }
    event EventHandler<bool> OfflineModeChanged;
    Task<T> ExecuteWithOfflineSupportAsync<T>(
        Func<Task<T>> onlineAction,
        Func<Task<T>> offlineAction);
    Task ExecuteWriteOperationAsync(Func<Task> action);
    void ShowOfflineToast();
}
