using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

/// <summary>
/// Employee management service implementation.
/// v1.2 - Operational MVP
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IApiClient _apiClient;
    private readonly IToastService _toastService;

    public EmployeeService(IApiClient apiClient, IToastService toastService)
    {
        _apiClient = apiClient;
        _toastService = toastService;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        try
        {
            var employees = await _apiClient.GetAsync<List<Employee>>("employee");
            return employees ?? new List<Employee>();
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return new List<Employee>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeeService] GetAllAsync error: {ex.Message}");
            await _toastService.ShowErrorAsync("Çalışanlar yüklenemedi");
            return new List<Employee>();
        }
    }

    public async Task<List<Employee>> GetActiveAsync()
    {
        var all = await GetAllAsync();
        return all.Where(e => e.IsActive).ToList();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        try
        {
            return await _apiClient.GetAsync<Employee>($"employee/{id}");
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeeService] GetByIdAsync error: {ex.Message}");
            await _toastService.ShowErrorAsync("Çalışan bilgisi yüklenemedi");
            return null;
        }
    }

    public async Task<Employee?> CreateAsync(string name)
    {
        try
        {
            // API expects Name field, not FirstName/LastName
            var request = new CreateEmployeeRequest
            {
                Name = name,
                IsActive = true,
                BusinessId = 1 // Default business for now
            };

            var created = await _apiClient.PostAsync<Employee>("employee", request);
            
            if (created != null)
            {
                await _toastService.ShowSuccessAsync("Çalışan eklendi");
            }
            
            return created;
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeeService] CreateAsync error: {ex.Message}");
            await _toastService.ShowErrorAsync("Çalışan eklenemedi");
            return null;
        }
    }

    public async Task<bool> UpdateAsync(int id, string name, bool isActive)
    {
        try
        {
            var request = new UpdateEmployeeRequest
            {
                Id = id,
                Name = name,
                IsActive = isActive,
                BusinessId = 1 // Default business for now
            };

            var success = await _apiClient.PutAsync($"employee/{id}", request);
            
            if (success)
            {
                await _toastService.ShowSuccessAsync("Çalışan güncellendi");
            }
            
            return success;
        }
        catch (ApiException ex)
        {
            await _toastService.ShowErrorAsync(ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[EmployeeService] UpdateAsync error: {ex.Message}");
            await _toastService.ShowErrorAsync("Çalışan güncellenemedi");
            return false;
        }
    }
}

/// <summary>
/// Request model for creating an employee.
/// </summary>
public class CreateEmployeeRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int BusinessId { get; set; }
}

/// <summary>
/// Request model for updating an employee.
/// </summary>
public class UpdateEmployeeRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int BusinessId { get; set; }
}
