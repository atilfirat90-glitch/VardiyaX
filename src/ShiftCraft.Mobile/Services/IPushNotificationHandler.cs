namespace ShiftCraft.Mobile.Services;

public interface IPushNotificationHandler
{
    Task RegisterDeviceAsync(string deviceToken);
    Task UnregisterDeviceAsync();
    Task<NotificationPreferences> GetPreferencesAsync();
    Task UpdatePreferencesAsync(NotificationPreferences preferences);
    void HandleNotificationReceived(PushNotificationPayload payload);
    void HandleNotificationTapped(PushNotificationPayload payload);
}

public class NotificationPreferences
{
    public bool ScheduleNotificationsEnabled { get; set; } = true;
    public bool ViolationNotificationsEnabled { get; set; } = true;
    public bool ShiftRemindersEnabled { get; set; } = true;
    public int ReminderHoursBefore { get; set; } = 24;
}

public class PushNotificationPayload
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
}

public class DeviceRegistrationRequest
{
    public string DeviceToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
}
