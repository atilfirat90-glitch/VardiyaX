using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

/// <summary>
/// v1.1: Refactored to use IApiClient for centralized HTTP handling.
/// </summary>
public class ApiService : IApiService
{
    private readonly IApiClient _apiClient;

    public ApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        try
        {
            return await _apiClient.GetAsync<List<Employee>>(ApiSettings.Endpoints.Employees) 
                ?? new List<Employee>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Çalışanlar yüklenemedi: {ex.Message}");
        }
    }

    public async Task<List<WeeklySchedule>> GetWeeklySchedulesAsync()
    {
        try
        {
            return await _apiClient.GetAsync<List<WeeklySchedule>>(ApiSettings.Endpoints.WeeklySchedules) 
                ?? new List<WeeklySchedule>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Vardiyalar yüklenemedi: {ex.Message}");
        }
    }

    public async Task<WeeklySchedule?> GetWeeklyScheduleAsync(int id)
    {
        try
        {
            return await _apiClient.GetAsync<WeeklySchedule>($"{ApiSettings.Endpoints.WeeklySchedules}/{id}");
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Vardiya yüklenemedi: {ex.Message}");
        }
    }

    public async Task<List<RuleViolation>> PublishScheduleAsync(int scheduleId)
    {
        try
        {
            return await _apiClient.PostAsync<List<RuleViolation>>(
                $"{ApiSettings.Endpoints.WeeklySchedules}/{scheduleId}/publish") 
                ?? new List<RuleViolation>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Vardiya yayınlanamadı: {ex.Message}");
        }
    }

    public async Task<List<RuleViolation>> GetRuleViolationsAsync()
    {
        try
        {
            // API returns { value: [...], Count: n } format
            var wrapper = await _apiClient.GetAsync<ApiListResponse<RuleViolation>>(ApiSettings.Endpoints.RuleViolations);
            return wrapper?.Value ?? new List<RuleViolation>();
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"İhlaller yüklenemedi: {ex.Message}");
        }
    }
}

// Wrapper class for API list responses
public class ApiListResponse<T>
{
    public List<T> Value { get; set; } = new();
    public int Count { get; set; }
}
