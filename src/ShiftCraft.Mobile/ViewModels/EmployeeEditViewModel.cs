using System.Windows.Input;
using ShiftCraft.Mobile.Services;

namespace ShiftCraft.Mobile.ViewModels;

/// <summary>
/// ViewModel for Employee Edit/Create screen.
/// v1.2 - Operational MVP
/// </summary>
public class EmployeeEditViewModel : BaseViewModel
{
    private readonly IEmployeeService _employeeService;
    private string _name = string.Empty;
    private bool _isActive = true;
    private bool _isEditMode;
    private string _errorMessage = string.Empty;
    private bool _hasError;

    public EmployeeEditViewModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
        Title = "Yeni Çalışan";
        SaveCommand = new Command(async () => await SaveAsync());
    }

    public int? EmployeeId { get; set; }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
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

    public ICommand SaveCommand { get; }

    public async Task LoadAsync()
    {
        if (EmployeeId.HasValue && EmployeeId.Value > 0)
        {
            // Edit mode
            IsEditMode = true;
            Title = "Çalışan Düzenle";
            IsBusy = true;

            try
            {
                var employee = await _employeeService.GetByIdAsync(EmployeeId.Value);
                if (employee != null)
                {
                    Name = employee.FullName;
                    IsActive = employee.IsActive;
                }
                else
                {
                    ErrorMessage = "Çalışan bulunamadı";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EmployeeEditViewModel] Load error: {ex.Message}");
                ErrorMessage = "Çalışan bilgisi yüklenemedi";
            }
            finally
            {
                IsBusy = false;
            }
        }
        else
        {
            // Create mode
            IsEditMode = false;
            Title = "Yeni Çalışan";
            Name = string.Empty;
            IsActive = true;
        }
    }

    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;

        // Validate
        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "Ad soyad boş olamaz";
            return;
        }

        if (Name.Trim().Length < 2)
        {
            ErrorMessage = "Ad soyad en az 2 karakter olmalı";
            return;
        }

        IsBusy = true;

        try
        {
            bool success;

            if (IsEditMode && EmployeeId.HasValue)
            {
                // Update existing employee
                success = await _employeeService.UpdateAsync(EmployeeId.Value, Name.Trim(), IsActive);
            }
            else
            {
                // Create new employee
                var created = await _employeeService.CreateAsync(Name.Trim());
                success = created != null;
            }

            if (success)
            {
                // Navigate back to list
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeeEditViewModel] Save error: {ex.Message}");
            ErrorMessage = "Kaydetme işlemi başarısız";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
