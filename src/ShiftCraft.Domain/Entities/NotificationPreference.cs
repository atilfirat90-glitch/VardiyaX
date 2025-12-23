namespace ShiftCraft.Domain.Entities;

public class NotificationPreference
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool ScheduleNotificationsEnabled { get; set; } = true;
    public bool ViolationNotificationsEnabled { get; set; } = true;
    public bool ShiftRemindersEnabled { get; set; } = true;
    public int ReminderHoursBefore { get; set; } = 24; // 1 or 24

    public User? User { get; set; }
}
