namespace ShiftCraft.Api.Models;

public class CreateTimeOffRequest
{
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = "PaidLeave";
    public string? Reason { get; set; }
    public int BusinessId { get; set; }
}

public class TimeOffDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? ResponseNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int BusinessId { get; set; }
}

public class RejectTimeOffRequest
{
    public string? ResponseNote { get; set; }
}
