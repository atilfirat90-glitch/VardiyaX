using System.Text.Json;
using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class CacheService : ICacheService
{
    private readonly string _cacheDir;
    private const string ScheduleCachePrefix = "schedule_";
    private const string MetadataFile = "cache_metadata.json";

    public CacheService()
    {
        _cacheDir = Path.Combine(FileSystem.AppDataDirectory, "cache");
        Directory.CreateDirectory(_cacheDir);
    }

    public async Task CacheScheduleAsync(WeeklySchedule schedule)
    {
        var key = GetScheduleKey(schedule.WeekStartDate);
        var filePath = Path.Combine(_cacheDir, $"{ScheduleCachePrefix}{key}.json");
        
        var json = JsonSerializer.Serialize(schedule);
        await File.WriteAllTextAsync(filePath, json);
        
        await SetLastSyncTimeAsync(DateTime.UtcNow);
    }

    public async Task<WeeklySchedule?> GetCachedScheduleAsync(DateTime weekStart)
    {
        var key = GetScheduleKey(weekStart);
        var filePath = Path.Combine(_cacheDir, $"{ScheduleCachePrefix}{key}.json");
        
        if (!File.Exists(filePath))
            return null;

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<WeeklySchedule>(json);
    }

    public async Task<List<WeeklySchedule>> GetAllCachedSchedulesAsync()
    {
        var schedules = new List<WeeklySchedule>();
        var files = Directory.GetFiles(_cacheDir, $"{ScheduleCachePrefix}*.json");
        
        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var schedule = JsonSerializer.Deserialize<WeeklySchedule>(json);
                if (schedule != null)
                    schedules.Add(schedule);
            }
            catch
            {
                // Skip corrupted files
            }
        }
        
        return schedules.OrderByDescending(s => s.WeekStartDate).ToList();
    }

    public Task ClearOldCacheAsync(int keepWeeks = 4)
    {
        var cutoffDate = DateTime.Today.AddDays(-keepWeeks * 7);
        var files = Directory.GetFiles(_cacheDir, $"{ScheduleCachePrefix}*.json");
        
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var dateStr = fileName.Replace(ScheduleCachePrefix, "");
            
            if (DateTime.TryParse(dateStr, out var fileDate) && fileDate < cutoffDate)
            {
                File.Delete(file);
            }
        }
        
        return Task.CompletedTask;
    }

    public async Task<DateTime?> GetLastSyncTimeAsync()
    {
        var metadata = await LoadMetadataAsync();
        return metadata.LastSyncTime;
    }

    public async Task SetLastSyncTimeAsync(DateTime syncTime)
    {
        var metadata = await LoadMetadataAsync();
        metadata.LastSyncTime = syncTime;
        await SaveMetadataAsync(metadata);
    }

    public Task<bool> HasCachedDataAsync()
    {
        var files = Directory.GetFiles(_cacheDir, $"{ScheduleCachePrefix}*.json");
        return Task.FromResult(files.Length > 0);
    }

    public Task ClearAllCacheAsync()
    {
        var files = Directory.GetFiles(_cacheDir);
        foreach (var file in files)
        {
            File.Delete(file);
        }
        return Task.CompletedTask;
    }

    private string GetScheduleKey(DateTime weekStart)
    {
        return weekStart.ToString("yyyy-MM-dd");
    }

    private async Task<CacheMetadata> LoadMetadataAsync()
    {
        var filePath = Path.Combine(_cacheDir, MetadataFile);
        
        if (!File.Exists(filePath))
            return new CacheMetadata();

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<CacheMetadata>(json) ?? new CacheMetadata();
    }

    private async Task SaveMetadataAsync(CacheMetadata metadata)
    {
        var filePath = Path.Combine(_cacheDir, MetadataFile);
        var json = JsonSerializer.Serialize(metadata);
        await File.WriteAllTextAsync(filePath, json);
    }

    private class CacheMetadata
    {
        public DateTime? LastSyncTime { get; set; }
    }
}
