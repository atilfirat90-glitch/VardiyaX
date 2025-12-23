namespace ShiftCraft.Application.Interfaces;

public interface IPushNotificationService
{
    Task SendSchedulePublishedNotificationAsync(int weeklyScheduleId, List<int> employeeIds, CancellationToken cancellationToken = default);
    Task SendViolationDetectedNotificationAsync(int violationId, List<int> managerUserIds, CancellationToken cancellationToken = default);
    Task ScheduleShiftReminderAsync(int shiftAssignmentId, int employeeId, DateTime shiftStart, CancellationToken cancellationToken = default);
    Task CancelShiftReminderAsync(int shiftAssignmentId, CancellationToken cancellationToken = default);
}

public class PushNotificationPayload
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // navigate_schedule, navigate_violations
    public Dictionary<string, string> Data { get; set; } = new();
}
