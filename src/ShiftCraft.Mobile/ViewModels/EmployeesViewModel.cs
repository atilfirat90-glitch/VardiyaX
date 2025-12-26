using System.Collections.ObjectModel;
using System.Windows.Input;
using ShiftCraft.Mobile.Models;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

public class EmployeesViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private string _errorMessage = string.Empty;

    public EmployeesViewModel(IApiService apiService, IAuthService authService)
    {
        _apiService = apiService;
        _authService = authService;
        Title = "Çalışanlar";
        Employees = new ObservableCollection<Employee>();
        RefreshCommand = new Command(async () => await LoadEmployeesAsync());
    }

    public ObservableCollection<Employee> Employees { get; }
    public ICommand RefreshCommand { get; }
    
    // Role-based visibility
    public bool CanEdit => _authService.IsManager;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public async Task LoadEmployeesAsync()
    {
        if (IsBusy) return;
        
        IsBusy = true;
        ErrorMessage = string.Empty;
        
        try
        {
            Employees.Clear();
            var employees = await _apiService.GetEmployeesAsync();
            foreach (var emp in employees)
            {
                Employees.Add(emp);
            }
        }
        catch (UnauthorizedAccessException)
        {
            await Shell.Current.GoToAsync("//login");
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeesViewModel] Network error: {ex}");
            ErrorMessage = "Bağlantı hatası. İnternet bağlantınızı kontrol edin.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeesViewModel] Load error: {ex}");
            ErrorMessage = "Çalışanlar yüklenirken hata oluştu";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
