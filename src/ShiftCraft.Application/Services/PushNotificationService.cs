using Microsoft.Extensions.Logging;
using ShiftCraft.Application.Interfaces;

namespace ShiftCraft.Application.Services;

public class PushNotificationService : IPushNotificationService
{
    private readonly IDeviceRegistrationRepository _deviceRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(
        IDeviceRegistrationRepository deviceRepository,
        INotificationPreferenceRepository preferenceRepository,
        ILogger<PushNotificationService> logger)
    {
        _deviceRepository = deviceRepository;
        _preferenceRepository = preferenceRepository;
        _logger = logger;
    }

    public async Task SendSchedulePublishedNotificationAsync(
        int weeklyScheduleId, 
        List<int> employeeIds, 
        CancellationToken cancellationToken = default)
    {
        // Get devices for employees who have schedule notifications enabled
        var devices = await _deviceRepository.GetActiveByUserIdsAsync(employeeIds, cancellationToken);
        
        foreach (var device in devices)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(device.UserId, cancellationToken);
            if (preference?.ScheduleNotificationsEnabled != false)
            {
                var payload = new PushNotificationPayload
                {
                    Title = "Yeni Program Yayınlandı",
                    Body = "Haftalık vardiya programınız güncellendi. Kontrol etmek için tıklayın.",
                    Action = "navigate_schedule",
                    Data = new Dictionary<string, string>
                    {
                        { "scheduleId", weeklyScheduleId.ToString() }
                    }
                };

                await SendPushNotificationAsync(device.DeviceToken, device.Platform, payload);
            }
        }

        _logger.LogInformation("Schedule published notification sent to {Count} devices for schedule {ScheduleId}", 
            devices.Count(), weeklyScheduleId);
    }

    public async Task SendViolationDetectedNotificationAsync(
        int violationId, 
        List<int> managerUserIds, 
        CancellationToken cancellationToken = default)
    {
        var devices = await _deviceRepository.GetActiveByUserIdsAsync(managerUserIds, cancellationToken);
        
        foreach (var device in devices)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(device.UserId, cancellationToken);
            if (preference?.ViolationNotificationsEnabled != false)
            {
                var payload = new PushNotificationPayload
                {
                    Title = "Kural İhlali Tespit Edildi",
                    Body = "Yeni bir kural ihlali tespit edildi. Detaylar için tıklayın.",
                    Action = "navigate_violations",
                    Data = new Dictionary<string, string>
                    {
                        { "violationId", violationId.ToString() }
                    }
                };

                await SendPushNotificationAsync(device.DeviceToken, device.Platform, payload);
            }
        }

        _logger.LogInformation("Violation notification sent to {Count} managers for violation {ViolationId}", 
            devices.Count(), violationId);
    }

    public async Task ScheduleShiftReminderAsync(
        int shiftAssignmentId, 
        int employeeId, 
        DateTime shiftStart, 
        CancellationToken cancellationToken = default)
    {
        var devices = await _deviceRepository.GetByUserIdAsync(employeeId, cancellationToken);
        
        foreach (var device in devices)
        {
            var preference = await _preferenceRepository.GetOrCreateByUserIdAsync(device.UserId, cancellationToken);
            if (preference.ShiftRemindersEnabled)
            {
                // In a real implementation, this would schedule the notification
                // using a background job scheduler like Hangfire or Azure Functions
                _logger.LogInformation(
                    "Shift reminder scheduled for assignment {AssignmentId}, employee {EmployeeId}, " +
                    "shift start {ShiftStart}, reminder {Hours}h before",
                    shiftAssignmentId, employeeId, shiftStart, preference.ReminderHoursBefore);
            }
        }

        await Task.CompletedTask;
    }

    public async Task CancelShiftReminderAsync(int shiftAssignmentId, CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would cancel scheduled notifications
        _logger.LogInformation("Shift reminder cancelled for assignment {AssignmentId}", shiftAssignmentId);
        await Task.CompletedTask;
    }

    private async Task SendPushNotificationAsync(string deviceToken, string platform, PushNotificationPayload payload)
    {
        // In a real implementation, this would send to FCM/APNS
        // For now, just log the notification
        _logger.LogInformation(
            "Push notification queued: Platform={Platform}, Token={Token}, Title={Title}",
            platform, deviceToken[..Math.Min(20, deviceToken.Length)] + "...", payload.Title);

        await Task.CompletedTask;
    }
}
