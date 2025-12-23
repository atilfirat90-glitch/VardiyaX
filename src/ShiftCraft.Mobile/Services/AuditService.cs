using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;

namespace ShiftCraft.Mobile.Services;

public class AuditService : IAuditService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public AuditService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(ApiSettings.BaseUrl);
        _authService = authService;
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
                    throw new UnauthorizedAccessException("Oturum süresi doldu.");
                case HttpStatusCode.Forbidden:
                    throw new Exception("Bu işlem için yetkiniz yok.");
                default:
                    throw new Exception($"Hata: {response.StatusCode}");
            }
        }
    }

    private string BuildQueryString(Dictionary<string, string?> parameters)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in parameters.Where(p => !string.IsNullOrEmpty(p.Value)))
        {
            query[param.Key] = param.Value;
        }
        return query.ToString() ?? string.Empty;
    }

    public async Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(LoginLogFilter filter)
    {
        SetAuthHeader();
        var queryParams = new Dictionary<string, string?>
        {
            ["username"] = filter.Username,
            ["from"] = filter.From?.ToString("yyyy-MM-dd"),
            ["to"] = filter.To?.ToString("yyyy-MM-dd"),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        var query = BuildQueryString(queryParams);
        var url = string.IsNullOrEmpty(query) ? "audit/login" : $"audit/login?{query}";
        
        var response = await _httpClient.GetAsync(url);
        await HandleResponse(response);
        return await response.Content.ReadFromJsonAsync<PagedResult<LoginLogDto>>() 
            ?? new PagedResult<LoginLogDto>();
    }

    public async Task<PagedResult<PublishLogDto>> GetPublishLogsAsync(PublishLogFilter filter)
    {
        SetAuthHeader();
        var queryParams = new Dictionary<string, string?>
        {
            ["publisher"] = filter.Publisher,
            ["from"] = filter.From?.ToString("yyyy-MM-dd"),
            ["to"] = filter.To?.ToString("yyyy-MM-dd"),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        var query = BuildQueryString(queryParams);
        var url = string.IsNullOrEmpty(query) ? "audit/publish" : $"audit/publish?{query}";
        
        var response = await _httpClient.GetAsync(url);
        await HandleResponse(response);
        return await response.Content.ReadFromJsonAsync<PagedResult<PublishLogDto>>() 
            ?? new PagedResult<PublishLogDto>();
    }

    public async Task<PagedResult<ViolationLogDto>> GetViolationHistoryAsync(ViolationFilter filter)
    {
        SetAuthHeader();
        var queryParams = new Dictionary<string, string?>
        {
            ["employeeId"] = filter.EmployeeId?.ToString(),
            ["ruleType"] = filter.RuleType,
            ["from"] = filter.From?.ToString("yyyy-MM-dd"),
            ["to"] = filter.To?.ToString("yyyy-MM-dd"),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        var query = BuildQueryString(queryParams);
        var url = string.IsNullOrEmpty(query) ? "audit/violations" : $"audit/violations?{query}";
        
        var response = await _httpClient.GetAsync(url);
        await HandleResponse(response);
        return await response.Content.ReadFromJsonAsync<PagedResult<ViolationLogDto>>() 
            ?? new PagedResult<ViolationLogDto>();
    }

    public async Task<ViolationTrendsDto> GetViolationTrendsAsync(DateTime from, DateTime to)
    {
        SetAuthHeader();
        var url = $"audit/violations/trends?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
        
        var response = await _httpClient.GetAsync(url);
        await HandleResponse(response);
        return await response.Content.ReadFromJsonAsync<ViolationTrendsDto>() 
            ?? new ViolationTrendsDto();
    }

    public async Task AcknowledgeViolationAsync(int violationId)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsync($"audit/violations/{violationId}/acknowledge", null);
        await HandleResponse(response);
    }
}
