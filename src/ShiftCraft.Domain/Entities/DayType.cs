namespace ShiftCraft.Domain.Entities;

public class DayType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;

    public ICollection<ScheduleDay> ScheduleDays { get; set; } = new List<ScheduleDay>();
    public ICollection<ShiftRequirement> ShiftRequirements { get; set; } = new List<ShiftRequirement>();
    public ICollection<CoreStaffRule> CoreStaffRules { get; set; } = new List<CoreStaffRule>();
}