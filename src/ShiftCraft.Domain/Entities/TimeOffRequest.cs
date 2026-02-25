namespace ShiftCraft.Domain.Entities;

public class TimeOffRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = "PaidLeave";
    public string Status { get; set; } = "Pending";
    public string? Reason { get; set; }
    public string? ResponseNote { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int BusinessId { get; set; }

    public Employee? Employee { get; set; }
}
