using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ShiftCraft.Mobile.Models;

namespace ShiftCraft.Mobile.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public ApiService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiSettings.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _authService = authService;
    }

    private async Task<T> ExecuteWithErrorHandling<T>(Func<Task<T>> action, T defaultValue)
    {
        try
        {
            // Check token expiry
            if (_authService.IsTokenExpired())
            {
                throw new UnauthorizedAccessException("Oturum süresi doldu. Lütfen tekrar giriş yapın.");
            }

            SetAuthHeader();
            return await action();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Sunucuya bağlanılamadı: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            throw new Exception("İstek zaman aşımına uğradı. Lütfen tekrar deneyin.");
        }
        catch (UnauthorizedAccessException)
        {
            // Token expired - trigger logout
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//login");
            throw;
        }
    }

    private void SetAuthHeader()
    {
        if (_authService.IsAuthenticated)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _authService.Token);
        }
    }

    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    await _authService.LogoutAsync();
                    await Shell.Current.GoToAsync("//login");
                    throw new UnauthorizedAccessException("Oturum süresi doldu.");
                case HttpStatusCode.Forbidden:
                    throw new Exception("Bu işlem için yetkiniz yok.");
                case HttpStatusCode.NotFound:
                    throw new Exception("Kayıt bulunamadı.");
                case HttpStatusCode.InternalServerError:
                    throw new Exception("Sunucu hatası. Lütfen daha sonra tekrar deneyin.");
                default:
                    throw new Exception($"Hata: {response.StatusCode}");
            }
        }
    }

    public async Task<List<Employee>> GetEmployeesAsync()
    {
        return await ExecuteWithErrorHandling(async () =>
        {
            var response = await _httpClient.GetAsync(ApiSettings.Endpoints.Employees);
            await HandleResponse(response);
            return await response.Content.ReadFromJsonAsync<List<Employee>>() ?? new List<Employee>();
        }, new List<Employee>());
    }

    public async Task<List<WeeklySchedule>> GetWeeklySchedulesAsync()
    {
        return await ExecuteWithErrorHandling(async () =>
        {
            var response = await _httpClient.GetAsync(ApiSettings.Endpoints.WeeklySchedules);
            await HandleResponse(response);
            return await response.Content.ReadFromJsonAsync<List<WeeklySchedule>>() ?? new List<WeeklySchedule>();
        }, new List<WeeklySchedule>());
    }

    public async Task<WeeklySchedule?> GetWeeklyScheduleAsync(int id)
    {
        return await ExecuteWithErrorHandling(async () =>
        {
            var response = await _httpClient.GetAsync($"{ApiSettings.Endpoints.WeeklySchedules}/{id}");
            await HandleResponse(response);
            return await response.Content.ReadFromJsonAsync<WeeklySchedule>();
        }, null);
    }

    public async Task<List<RuleViolation>> PublishScheduleAsync(int scheduleId)
    {
        return await ExecuteWithErrorHandling(async () =>
        {
            var response = await _httpClient.PostAsync($"{ApiSettings.Endpoints.WeeklySchedules}/{scheduleId}/publish", null);
            await HandleResponse(response);
            return await response.Content.ReadFromJsonAsync<List<RuleViolation>>() ?? new List<RuleViolation>();
        }, new List<RuleViolation>());
    }

    public async Task<List<RuleViolation>> GetRuleViolationsAsync()
    {
        return await ExecuteWithErrorHandling(async () =>
        {
            var response = await _httpClient.GetAsync(ApiSettings.Endpoints.RuleViolations);
            await HandleResponse(response);
            return await response.Content.ReadFromJsonAsync<List<RuleViolation>>() ?? new List<RuleViolation>();
        }, new List<RuleViolation>());
    }
}
