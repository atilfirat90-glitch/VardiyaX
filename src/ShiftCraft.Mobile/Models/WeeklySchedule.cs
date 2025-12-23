namespace ShiftCraft.Mobile.Models;

public class WeeklySchedule
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public int Status { get; set; } // 0=Draft, 1=Published
    public List<ScheduleDay>? ScheduleDays { get; set; }
    
    public string StatusText => Status == 0 ? "Draft" : "Published";
    public string WeekRange => $"{WeekStartDate:dd MMM} - {WeekEndDate:dd MMM yyyy}";
}

public class ScheduleDay
{
    public int Id { get; set; }
    public int WeeklyScheduleId { get; set; }
    public DateTime Date { get; set; }
    public int DayTypeId { get; set; }
    public List<ShiftAssignment>? ShiftAssignments { get; set; }
}

public class ShiftAssignment
{
    public int Id { get; set; }
    public int ScheduleDayId { get; set; }
    public int EmployeeId { get; set; }
    public int ShiftTemplateId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
