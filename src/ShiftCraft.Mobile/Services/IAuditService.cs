namespace ShiftCraft.Mobile.Services;

public interface IAuditService
{
    Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(LoginLogFilter filter);
    Task<PagedResult<PublishLogDto>> GetPublishLogsAsync(PublishLogFilter filter);
    Task<PagedResult<ViolationLogDto>> GetViolationHistoryAsync(ViolationFilter filter);
    Task<ViolationTrendsDto> GetViolationTrendsAsync(DateTime from, DateTime to);
    Task AcknowledgeViolationAsync(int violationId);
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class LoginLogDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string? FailureReason { get; set; }
}

public class PublishLogDto
{
    public int Id { get; set; }
    public string PublisherUsername { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime WeekStartDate { get; set; }
    public int AffectedEmployeeCount { get; set; }
}

public class ViolationLogDto
{
    public int Id { get; set; }
    public DateTime ViolationDate { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Severity { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
}

public class ViolationTrendsDto
{
    public List<TrendPoint> WeeklyTrends { get; set; } = new();
    public List<TrendPoint> MonthlyTrends { get; set; } = new();
}

public class TrendPoint
{
    public DateTime Period { get; set; }
    public int Count { get; set; }
}

public class LoginLogFilter
{
    public string? Username { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PublishLogFilter
{
    public string? Publisher { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ViolationFilter
{
    public int? EmployeeId { get; set; }
    public string? RuleType { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
