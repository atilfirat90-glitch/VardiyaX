namespace ShiftCraft.Api.Models;

public class NotificationPreferencesDto
{
    public bool ScheduleNotificationsEnabled { get; set; } = true;
    public bool ViolationNotificationsEnabled { get; set; } = true;
    public bool ShiftRemindersEnabled { get; set; } = true;
    public int ReminderHoursBefore { get; set; } = 24;
}

public class RegisterDeviceRequest
{
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // Android, iOS, Windows
}
