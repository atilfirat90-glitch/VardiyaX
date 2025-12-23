using ShiftCraft.Domain.Enums;

namespace ShiftCraft.Domain.Entities;

public class RuleViolation
{
    public int Id { get; set; }
    public int WeeklyScheduleId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime ViolationDate { get; set; }
    public RuleCode RuleCode { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }

    public WeeklySchedule? WeeklySchedule { get; set; }
    public Employee? Employee { get; set; }
}