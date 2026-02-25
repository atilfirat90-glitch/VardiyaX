using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

/// <summary>
/// ViewModel for Shift Create screen.
/// v1.4 - Shift Create
/// </summary>
public class ShiftCreateViewModel : BaseViewModel
{
    private readonly IShiftService _shiftService;
    private readonly IEmployeeService _employeeService;
    private readonly IAuthService _authService;
    private Employee? _selectedEmployee;
    private DateTime _selectedDate = DateTime.Today;
    private TimeSpan _startTime = new(8, 0, 0);
    private TimeSpan _endTime = new(16, 0, 0);
    private string _errorMessage = string.Empty;
    private bool _hasError;
    private string _successMessage = string.Empty;
    private bool _hasSuccess;

    public ShiftCreateViewModel(IShiftService shiftService, IEmployeeService employeeService, IAuthService authService)
    {
        _shiftService = shiftService;
        _employeeService = employeeService;
        _authService = authService;
        Title = "Vardiya Oluştur";
        Employees = new ObservableCollection<Employee>();

        LoadEmployeesCommand = new Command(async () => await LoadEmployeesAsync());
        SaveCommand = new Command(async () => await SaveAsync());
    }

    public ObservableCollection<Employee> Employees { get; }

    public ICommand LoadEmployeesCommand { get; }
    public ICommand SaveCommand { get; }

    public Employee? SelectedEmployee
    {
        get => _selectedEmployee;
        set => SetProperty(ref _selectedEmployee, value);
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set => SetProperty(ref _selectedDate, value);
    }

    public TimeSpan StartTime
    {
        get => _startTime;
        set => SetProperty(ref _startTime, value);
    }

    public TimeSpan EndTime
    {
        get => _endTime;
        set => SetProperty(ref _endTime, value);
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

    public string SuccessMessage
    {
        get => _successMessage;
        set
        {
            SetProperty(ref _successMessage, value);
            HasSuccess = !string.IsNullOrEmpty(value);
        }
    }

    public bool HasSuccess
    {
        get => _hasSuccess;
        set => SetProperty(ref _hasSuccess, value);
    }

    public async Task LoadEmployeesAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            Employees.Clear();
            var employees = await _employeeService.GetActiveAsync();
            foreach (var emp in employees.OrderBy(e => e.FullName))
            {
                Employees.Add(emp);
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftCreateViewModel] LoadEmployees error: {ex.Message}");
            ErrorMessage = "Çalışanlar yüklenemedi";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (SelectedEmployee == null)
        {
            ErrorMessage = "Lütfen bir çalışan seçin";
            return;
        }

        if (EndTime <= StartTime)
        {
            ErrorMessage = "Bitiş saati başlangıç saatinden sonra olmalıdır";
            return;
        }

        IsBusy = true;

        try
        {
            var request = new CreateShiftRequest
            {
                EmployeeId = SelectedEmployee.Id,
                Date = SelectedDate,
                StartTime = StartTime,
                EndTime = EndTime,
                BusinessId = 1
            };

            var result = await _shiftService.CreateShiftAsync(request);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Başarılı", "Vardiya başarıyla oluşturuldu", "Tamam");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ShiftCreateViewModel] Save error: {ex.Message}");
            ErrorMessage = "Vardiya oluşturulurken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
