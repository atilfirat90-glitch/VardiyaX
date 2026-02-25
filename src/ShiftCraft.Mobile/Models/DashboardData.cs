namespace ShiftCraft.Mobile.Models;

public class DashboardData
{
    public int TodayShiftCount { get; set; }
    public int ActiveEmployeeCount { get; set; }
    public int PendingSwapCount { get; set; }
    public int PendingTimeOffCount { get; set; }
    public double WeeklyHoursTotal { get; set; }
    public List<ShiftResponseDto> UpcomingShifts { get; set; } = new();
}
