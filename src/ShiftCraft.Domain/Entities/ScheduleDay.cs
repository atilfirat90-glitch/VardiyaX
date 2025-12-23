namespace ShiftCraft.Domain.Entities;

public class ScheduleDay
{
    public int Id { get; set; }
    public int WeeklyScheduleId { get; set; }
    public DateTime Date { get; set; }
    public int DayTypeId { get; set; }

    public WeeklySchedule? WeeklySchedule { get; set; }
    public DayType? DayType { get; set; }
    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
}