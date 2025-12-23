using ShiftCraft.Domain.Enums;

namespace ShiftCraft.Domain.Entities;

public class WeeklySchedule
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public ScheduleStatus Status { get; set; }

    public Business? Business { get; set; }
    public ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();
    public ICollection<RuleViolation> RuleViolations { get; set; } = new List<RuleViolation>();
}