using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly IDashboardService _dashboardService;
    private readonly IAuthService _authService;
    private int _todayShiftCount;
    private int _activeEmployeeCount;
    private int _pendingSwapCount;
    private int _pendingTimeOffCount;
    private double _weeklyHoursTotal;

    public DashboardViewModel(IDashboardService dashboardService, IAuthService authService)
    {
        _dashboardService = dashboardService;
        _authService = authService;
        Title = "Ana Panel";
        UpcomingShifts = new ObservableCollection<ShiftResponseDto>();
        LoadCommand = new Command(async () => await LoadAsync());
    }

    public ICommand LoadCommand { get; }
    public ObservableCollection<ShiftResponseDto> UpcomingShifts { get; }

    public int TodayShiftCount
    {
        get => _todayShiftCount;
        set => SetProperty(ref _todayShiftCount, value);
    }

    public int ActiveEmployeeCount
    {
        get => _activeEmployeeCount;
        set => SetProperty(ref _activeEmployeeCount, value);
    }

    public int PendingSwapCount
    {
        get => _pendingSwapCount;
        set => SetProperty(ref _pendingSwapCount, value);
    }

    public int PendingTimeOffCount
    {
        get => _pendingTimeOffCount;
        set => SetProperty(ref _pendingTimeOffCount, value);
    }

    public double WeeklyHoursTotal
    {
        get => _weeklyHoursTotal;
        set => SetProperty(ref _weeklyHoursTotal, value);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var data = await _dashboardService.GetDashboardAsync(1);
            if (data != null)
            {
                TodayShiftCount = data.TodayShiftCount;
                ActiveEmployeeCount = data.ActiveEmployeeCount;
                PendingSwapCount = data.PendingSwapCount;
                PendingTimeOffCount = data.PendingTimeOffCount;
                WeeklyHoursTotal = data.WeeklyHoursTotal;

                UpcomingShifts.Clear();
                foreach (var shift in data.UpcomingShifts)
                {
                    UpcomingShifts.Add(shift);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DashboardViewModel] Load error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
