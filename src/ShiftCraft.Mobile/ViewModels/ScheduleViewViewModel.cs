using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

/// <summary>
/// ViewModel for Schedule View screen (daily/weekly).
/// v1.4 - Schedule View
/// </summary>
public class ScheduleViewViewModel : BaseViewModel
{
    private readonly IShiftService _shiftService;
    private readonly IAuthService _authService;
    private string _viewMode = "Daily";
    private DateTime _selectedDate = DateTime.Today;
    private string _errorMessage = string.Empty;
    private bool _isEmpty;

    public ScheduleViewViewModel(IShiftService shiftService, IAuthService authService)
    {
        _shiftService = shiftService;
        _authService = authService;
        Title = "Program Görüntüle";
        ShiftGroups = new ObservableCollection<ShiftGroup>();

        DailyCommand = new Command(() => SetViewMode("Daily"));
        WeeklyCommand = new Command(() => SetViewMode("Weekly"));
        PreviousDayCommand = new Command(async () => await NavigateDay(-1));
        NextDayCommand = new Command(async () => await NavigateDay(1));
        TodayCommand = new Command(async () => await GoToToday());
        RefreshCommand = new Command(async () => await LoadShiftsAsync());
    }

    public ObservableCollection<ShiftGroup> ShiftGroups { get; }

    public ICommand DailyCommand { get; }
    public ICommand WeeklyCommand { get; }
    public ICommand PreviousDayCommand { get; }
    public ICommand NextDayCommand { get; }
    public ICommand TodayCommand { get; }
    public ICommand RefreshCommand { get; }

    public string ViewMode
    {
        get => _viewMode;
        set => SetProperty(ref _viewMode, value);
    }

    public bool IsDailyMode => ViewMode == "Daily";
    public bool IsWeeklyMode => ViewMode == "Weekly";

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
                OnPropertyChanged(nameof(SelectedDateText));
        }
    }

    public string SelectedDateText => SelectedDate.ToString("dd MMMM yyyy, dddd");

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsEmpty
    {
        get => _isEmpty;
        set => SetProperty(ref _isEmpty, value);
    }

    public async Task LoadShiftsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        IsEmpty = false;

        try
        {
            List<ShiftResponseDto> shifts;
            int businessId = 1;

            if (ViewMode == "Daily")
            {
                shifts = await _shiftService.GetBusinessShiftsByDateAsync(businessId, SelectedDate);
            }
            else
            {
                var weekStart = SelectedDate.AddDays(-(int)SelectedDate.DayOfWeek + (int)DayOfWeek.Monday);
                if (SelectedDate.DayOfWeek == DayOfWeek.Sunday)
                    weekStart = weekStart.AddDays(-7);
                var weekEnd = weekStart.AddDays(6);

                shifts = await _shiftService.GetShiftsByDateRangeAsync(0, weekStart, weekEnd);
            }

            ShiftGroups.Clear();

            if (shifts.Count == 0)
            {
                IsEmpty = true;
            }
            else
            {
                var grouped = shifts
                    .GroupBy(s => s.Date.Date)
                    .OrderBy(g => g.Key);

                foreach (var group in grouped)
                {
                    var header = group.Key.ToString("dd MMM yyyy, dddd");
                    ShiftGroups.Add(new ShiftGroup(header, group.OrderBy(s => s.StartTime)));
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ScheduleViewViewModel] Load error: {ex}");
            ErrorMessage = "Vardiyalar yüklenirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SetViewMode(string mode)
    {
        ViewMode = mode;
        OnPropertyChanged(nameof(IsDailyMode));
        OnPropertyChanged(nameof(IsWeeklyMode));
        _ = LoadShiftsAsync();
    }

    private async Task NavigateDay(int offset)
    {
        SelectedDate = ViewMode == "Weekly"
            ? SelectedDate.AddDays(offset * 7)
            : SelectedDate.AddDays(offset);
        await LoadShiftsAsync();
    }

    private async Task GoToToday()
    {
        SelectedDate = DateTime.Today;
        await LoadShiftsAsync();
    }
}
