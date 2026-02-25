namespace ShiftCraft.Api.Models;

public class CreateSwapRequest
{
    public int RequesterShiftId { get; set; }
    public int? TargetEmployeeId { get; set; }
    public string? Reason { get; set; }
}

public class ShiftSwapDto
{
    public int Id { get; set; }
    public int RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public int RequesterShiftId { get; set; }
    public int? TargetEmployeeId { get; set; }
    public string? TargetEmployeeName { get; set; }
    public int? TargetShiftId { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int BusinessId { get; set; }
}
