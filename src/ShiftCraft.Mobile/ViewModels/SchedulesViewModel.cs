using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class SchedulesViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private readonly ICacheService _cacheService;
    private readonly IOfflineModeService _offlineModeService;
    private string _errorMessage = string.Empty;
    private bool _isOffline;
    private DateTime? _lastSyncTime;

    public SchedulesViewModel(
        IApiService apiService, 
        IAuthService authService,
        ICacheService cacheService,
        IOfflineModeService offlineModeService)
    {
        _apiService = apiService;
        _authService = authService;
        _cacheService = cacheService;
        _offlineModeService = offlineModeService;
        
        Title = "Vardiyalar";
        Schedules = new ObservableCollection<WeeklySchedule>();
        RefreshCommand = new Command(async () => await LoadSchedulesAsync());
        PublishCommand = new Command<WeeklySchedule>(async (s) => await PublishScheduleAsync(s));
        
        // Listen for connectivity changes
        _offlineModeService.OfflineModeChanged += OnOfflineModeChanged;
        _isOffline = _offlineModeService.IsOfflineMode;
    }

    public ObservableCollection<WeeklySchedule> Schedules { get; }
    public ICommand RefreshCommand { get; }
    public ICommand PublishCommand { get; }
    
    // Role-based visibility
    public bool CanPublish => _authService.IsManager && !IsOffline;
    public bool IsReadOnly => _authService.IsWorker;

    public bool IsOffline
    {
        get => _isOffline;
        set
        {
            if (SetProperty(ref _isOffline, value))
            {
                OnPropertyChanged(nameof(CanPublish));
                OnPropertyChanged(nameof(OfflineStatusText));
            }
        }
    }

    public DateTime? LastSyncTime
    {
        get => _lastSyncTime;
        set => SetProperty(ref _lastSyncTime, value);
    }

    public string OfflineStatusText => IsOffline 
        ? $"Çevrimdışı - Son senkronizasyon: {LastSyncTime?.ToString("dd.MM HH:mm") ?? "Bilinmiyor"}"
        : string.Empty;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public async Task LoadSchedulesAsync()
    {
        if (IsBusy) return;
        
        IsBusy = true;
        ErrorMessage = string.Empty;
        
        try
        {
            var schedules = await _offlineModeService.ExecuteWithOfflineSupportAsync(
                async () =>
                {
                    // Online: fetch from API and cache
                    var data = await _apiService.GetWeeklySchedulesAsync();
                    foreach (var schedule in data)
                    {
                        await _cacheService.CacheScheduleAsync(schedule);
                    }
                    await _cacheService.SetLastSyncTimeAsync(DateTime.UtcNow);
                    return data;
                },
                async () =>
                {
                    // Offline: load from cache
                    return await _cacheService.GetAllCachedSchedulesAsync();
                });

            Schedules.Clear();
            foreach (var schedule in schedules)
            {
                Schedules.Add(schedule);
            }

            LastSyncTime = await _cacheService.GetLastSyncTimeAsync();
        }
        catch (UnauthorizedAccessException)
        {
            // Already handled by ApiService
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PublishScheduleAsync(WeeklySchedule? schedule)
    {
        if (schedule == null) return;
        
        // Check permission
        if (!_authService.IsManager)
        {
            await Shell.Current.DisplayAlert("Yetki Hatası", "Bu işlem için yönetici yetkisi gerekli.", "Tamam");
            return;
        }

        // Check offline mode - show toast instead of modal
        if (_offlineModeService.IsOfflineMode)
        {
            _offlineModeService.ShowOfflineToast();
            return;
        }
        
        var confirm = await Shell.Current.DisplayAlert("Yayınla", 
            $"{schedule.WeekRange} tarihli vardiyayı yayınlamak istiyor musunuz?", "Evet", "Hayır");
        
        if (!confirm) return;

        IsBusy = true;
        try
        {
            await _offlineModeService.ExecuteWriteOperationAsync(async () =>
            {
                var violations = await _apiService.PublishScheduleAsync(schedule.Id);
                if (violations.Any())
                {
                    await Shell.Current.DisplayAlert("Uyarı", 
                        $"Vardiya yayınlandı ancak {violations.Count} kural ihlali tespit edildi.", "Tamam");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Başarılı", "Vardiya başarıyla yayınlandı.", "Tamam");
                }
            });
            await LoadSchedulesAsync();
        }
        catch (OfflineException)
        {
            // Toast already shown by OfflineModeService
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hata", ex.Message, "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async void OnOfflineModeChanged(object? sender, bool isOffline)
    {
        IsOffline = isOffline;
        
        // Auto-sync when coming back online
        if (!isOffline)
        {
            await LoadSchedulesAsync();
        }
    }
}
