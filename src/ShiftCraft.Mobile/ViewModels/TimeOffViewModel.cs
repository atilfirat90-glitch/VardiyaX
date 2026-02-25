using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class TimeOffViewModel : BaseViewModel
{
    private readonly ITimeOffService _timeOffService;
    private readonly IAuthService _authService;
    private DateTime _startDate = DateTime.Today;
    private DateTime _endDate = DateTime.Today.AddDays(1);
    private string _selectedType = "PaidLeave";
    private string _reason = string.Empty;
    private bool _showForm;
    private string _errorMessage = string.Empty;
    private bool _hasError;

    public TimeOffViewModel(ITimeOffService timeOffService, IAuthService authService)
    {
        _timeOffService = timeOffService;
        _authService = authService;
        Title = "İzin Talepleri";
        TimeOffRequests = new ObservableCollection<TimeOffRequestModel>();
        LeaveTypes = new List<string> { "PaidLeave", "SickLeave", "Unpaid" };

        LoadCommand = new Command(async () => await LoadAsync());
        CreateCommand = new Command(async () => await CreateAsync());
        ApproveCommand = new Command<int>(async (id) => await ApproveAsync(id));
        RejectCommand = new Command<int>(async (id) => await RejectAsync(id));
        ToggleFormCommand = new Command(() => ShowForm = !ShowForm);
    }

    public ICommand LoadCommand { get; }
    public ICommand CreateCommand { get; }
    public ICommand ApproveCommand { get; }
    public ICommand RejectCommand { get; }
    public ICommand ToggleFormCommand { get; }
    public ObservableCollection<TimeOffRequestModel> TimeOffRequests { get; }
    public List<string> LeaveTypes { get; }

    public bool IsManager => _authService.IsManager;

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public string SelectedType
    {
        get => _selectedType;
        set => SetProperty(ref _selectedType, value);
    }

    public string Reason
    {
        get => _reason;
        set => SetProperty(ref _reason, value);
    }

    public bool ShowForm
    {
        get => _showForm;
        set => SetProperty(ref _showForm, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            HasError = !string.IsNullOrEmpty(value);
        }
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            TimeOffRequests.Clear();
            List<TimeOffRequestModel> requests;

            if (_authService.IsManager)
                requests = await _timeOffService.GetPendingByBusinessAsync(1);
            else
                requests = await _timeOffService.GetByEmployeeAsync(1);

            foreach (var req in requests)
            {
                TimeOffRequests.Add(req);
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimeOffViewModel] Load error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateAsync()
    {
        ErrorMessage = string.Empty;

        if (EndDate <= StartDate)
        {
            ErrorMessage = "Bitiş tarihi başlangıç tarihinden sonra olmalıdır";
            return;
        }

        IsBusy = true;

        try
        {
            var request = new CreateTimeOffRequestModel
            {
                EmployeeId = 1,
                StartDate = StartDate,
                EndDate = EndDate,
                Type = SelectedType,
                Reason = Reason,
                BusinessId = 1
            };

            var result = await _timeOffService.CreateAsync(request);
            if (result != null)
            {
                ShowForm = false;
                Reason = string.Empty;
                await LoadAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TimeOffViewModel] Create error: {ex.Message}");
            ErrorMessage = "İzin talebi oluşturulamadı";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ApproveAsync(int id)
    {
        var confirm = await Shell.Current.DisplayAlert("Onay", "Bu izin talebini onaylamak istiyor musunuz?", "Evet", "Hayır");
        if (!confirm) return;

        var result = await _timeOffService.ApproveAsync(id);
        if (result) await LoadAsync();
    }

    private async Task RejectAsync(int id)
    {
        var confirm = await Shell.Current.DisplayAlert("Reddet", "Bu izin talebini reddetmek istiyor musunuz?", "Evet", "Hayır");
        if (!confirm) return;

        var result = await _timeOffService.RejectAsync(id);
        if (result) await LoadAsync();
    }
}
