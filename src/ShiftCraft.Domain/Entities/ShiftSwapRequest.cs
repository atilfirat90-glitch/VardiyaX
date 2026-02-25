namespace ShiftCraft.Domain.Entities;

public class ShiftSwapRequest
{
    public int Id { get; set; }
    public int RequesterId { get; set; }
    public int RequesterShiftId { get; set; }
    public int? TargetEmployeeId { get; set; }
    public int? TargetShiftId { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public int BusinessId { get; set; }

    public Employee? Requester { get; set; }
    public ShiftAssignment? RequesterShift { get; set; }
    public Employee? TargetEmployee { get; set; }
    public ShiftAssignment? TargetShift { get; set; }
}
