using System.Web;

namespace ShiftCraft.Mobile.Services;

/// <summary>
/// v1.1: Refactored to use IApiClient for centralized HTTP handling.
/// </summary>
public class AuditService : IAuditService
{
    private readonly IApiClient _apiClient;

    public AuditService(IApiClient apiClient)
    {
        _apiClient = apiClient;
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
        var queryParams = new Dictionary<string, string?>
        {
            ["username"] = filter.Username,
            ["from"] = filter.From?.ToString("yyyy-MM-dd"),
            ["to"] = filter.To?.ToString("yyyy-MM-dd"),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        var query = BuildQueryString(queryParams);
        var endpoint = string.IsNullOrEmpty(query) ? "audit/login" : $"audit/login?{query}";
        
        return await _apiClient.GetAsync<PagedResult<LoginLogDto>>(endpoint) 
            ?? new PagedResult<LoginLogDto>();
    }

    public async Task<PagedResult<PublishLogDto>> GetPublishLogsAsync(PublishLogFilter filter)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["publisher"] = filter.Publisher,
            ["from"] = filter.From?.ToString("yyyy-MM-dd"),
            ["to"] = filter.To?.ToString("yyyy-MM-dd"),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        var query = BuildQueryString(queryParams);
        var endpoint = string.IsNullOrEmpty(query) ? "audit/publish" : $"audit/publish?{query}";
        
        return await _apiClient.GetAsync<PagedResult<PublishLogDto>>(endpoint) 
            ?? new PagedResult<PublishLogDto>();
    }

    public async Task<PagedResult<ViolationLogDto>> GetViolationHistoryAsync(ViolationFilter filter)
    {
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
        var endpoint = string.IsNullOrEmpty(query) ? "audit/violations" : $"audit/violations?{query}";
        
        return await _apiClient.GetAsync<PagedResult<ViolationLogDto>>(endpoint) 
            ?? new PagedResult<ViolationLogDto>();
    }

    public async Task<ViolationTrendsDto> GetViolationTrendsAsync(DateTime from, DateTime to)
    {
        var endpoint = $"audit/violations/trends?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
        return await _apiClient.GetAsync<ViolationTrendsDto>(endpoint) 
            ?? new ViolationTrendsDto();
    }

    public async Task AcknowledgeViolationAsync(int violationId)
    {
        await _apiClient.PostAsync($"audit/violations/{violationId}/acknowledge");
    }
}
