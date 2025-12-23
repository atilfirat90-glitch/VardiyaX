namespace ShiftCraft.Api.Models;

public class LoginLogDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
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
    public int EmployeeId { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
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

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
