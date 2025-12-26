using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

/// <summary>
/// ViewModel for Employee Management screen.
/// v1.2 - Operational MVP
/// </summary>
public class EmployeeManageViewModel : BaseViewModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IAuthService _authService;
    private bool _showAccessDenied;

    public EmployeeManageViewModel(IEmployeeService employeeService, IAuthService authService)
    {
        _employeeService = employeeService;
        _authService = authService;
        Title = "Çalışan Yönetimi";
        Employees = new ObservableCollection<Employee>();
        
        RefreshCommand = new Command(async () => await LoadEmployeesAsync());
        AddEmployeeCommand = new Command(async () => await AddEmployeeAsync());
        EditEmployeeCommand = new Command<Employee>(async (emp) => await EditEmployeeAsync(emp));
    }

    public ObservableCollection<Employee> Employees { get; }
    
    public ICommand RefreshCommand { get; }
    public ICommand AddEmployeeCommand { get; }
    public ICommand EditEmployeeCommand { get; }

    public bool ShowAccessDenied
    {
        get => _showAccessDenied;
        set => SetProperty(ref _showAccessDenied, value);
    }

    public async Task LoadEmployeesAsync()
    {
        // Check role access
        if (!_authService.IsManager)
        {
            ShowAccessDenied = true;
            return;
        }
        
        ShowAccessDenied = false;
        
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            Employees.Clear();
            var employees = await _employeeService.GetAllAsync();
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
            System.Diagnostics.Debug.WriteLine($"[EmployeeManageViewModel] Load error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddEmployeeAsync()
    {
        if (!_authService.IsManager)
        {
            ShowAccessDenied = true;
            return;
        }

        // Navigate to edit page in create mode
        await Shell.Current.GoToAsync("employeeedit");
    }

    private async Task EditEmployeeAsync(Employee? employee)
    {
        if (employee == null) return;
        
        if (!_authService.IsManager)
        {
            ShowAccessDenied = true;
            return;
        }

        // Navigate to edit page with employee ID
        await Shell.Current.GoToAsync($"employeeedit?id={employee.Id}");
    }
}
