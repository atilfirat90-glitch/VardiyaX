namespace ShiftCraft.Domain.Entities;

public class WorkRule
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public bool IsSevenDaysOpen { get; set; }
    public int MaxDailyMinutes { get; set; }
    public int MinWeeklyOffDays { get; set; }

    public Business? Business { get; set; }
}