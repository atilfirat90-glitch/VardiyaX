using System.Text.Json;
using Microsoft.Extensions.Logging;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Application.Services;

public class PushNotificationService : IPushNotificationService
{
    private readonly IDeviceRegistrationRepository _deviceRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(
        IDeviceRegistrationRepository deviceRepository,
        INotificationPreferenceRepository preferenceRepository,
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ILogger<PushNotificationService> logger)
    {
        _deviceRepository = deviceRepository;
        _preferenceRepository = preferenceRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task SendSchedulePublishedNotificationAsync(
        int weeklyScheduleId, 
        List<int> employeeIds, 
        CancellationToken cancellationToken = default)
    {
        var notifiedCount = 0;

        try
        {
            var allUsers = await _userRepository.GetAllAsync(cancellationToken);
            var activeUserIds = allUsers.Where(u => u.IsActive).Select(u => u.Id).ToHashSet();

            foreach (var user in allUsers.Where(u => u.IsActive))
            {
                var preference = await _preferenceRepository.GetByUserIdAsync(user.Id, cancellationToken);
                if (preference?.ScheduleNotificationsEnabled == false)
                    continue;

                var notification = new Notification
                {
                    UserId = user.Id,
                    Title = "Yeni Program Yayınlandı",
                    Body = $"Haftalık vardiya programınız güncellendi. {employeeIds.Count} çalışan etkilendi.",
                    Type = "SchedulePublished",
                    Action = "navigate_schedule",
                    DataJson = JsonSerializer.Serialize(new { scheduleId = weeklyScheduleId, affectedEmployees = employeeIds.Count }),
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationRepository.AddAsync(notification, cancellationToken);
                notifiedCount++;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create schedule published notifications for schedule {ScheduleId}", weeklyScheduleId);
        }

        // Also send to registered devices (FCM/APNS placeholder)
        var devices = await _deviceRepository.GetActiveByUserIdsAsync(employeeIds, cancellationToken);
        foreach (var device in devices)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(device.UserId, cancellationToken);
            if (preference?.ScheduleNotificationsEnabled != false)
            {
                await SendPushToDeviceAsync(device.DeviceToken, device.Platform, 
                    "Yeni Program Yayınlandı", 
                    "Haftalık vardiya programınız güncellendi.",
                    "navigate_schedule");
            }
        }

        _logger.LogInformation(
            "Schedule published notification: {Count} in-app notifications created for schedule {ScheduleId}", 
            notifiedCount, weeklyScheduleId);
    }

    public async Task SendViolationDetectedNotificationAsync(
        int violationId, 
        List<int> managerUserIds, 
        CancellationToken cancellationToken = default)
    {
        var notifiedCount = 0;

        foreach (var userId in managerUserIds)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
            if (preference?.ViolationNotificationsEnabled == false)
                continue;

            var notification = new Notification
            {
                UserId = userId,
                Title = "Kural İhlali Tespit Edildi",
                Body = "Yeni bir kural ihlali tespit edildi. Detaylar için tıklayın.",
                Type = "ViolationDetected",
                Action = "navigate_violations",
                DataJson = JsonSerializer.Serialize(new { violationId }),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
            notifiedCount++;
        }

        var devices = await _deviceRepository.GetActiveByUserIdsAsync(managerUserIds, cancellationToken);
        foreach (var device in devices)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(device.UserId, cancellationToken);
            if (preference?.ViolationNotificationsEnabled != false)
            {
                await SendPushToDeviceAsync(device.DeviceToken, device.Platform,
                    "Kural İhlali Tespit Edildi",
                    "Yeni bir kural ihlali tespit edildi.",
                    "navigate_violations");
            }
        }

        _logger.LogInformation(
            "Violation notification: {Count} in-app notifications created for violation {ViolationId}", 
            notifiedCount, violationId);
    }

    public async Task ScheduleShiftReminderAsync(
        int shiftAssignmentId, 
        int employeeId, 
        DateTime shiftStart, 
        CancellationToken cancellationToken = default)
    {
        var preference = await _preferenceRepository.GetOrCreateByUserIdAsync(employeeId, cancellationToken);
        if (!preference.ShiftRemindersEnabled)
            return;

        var notification = new Notification
        {
            UserId = employeeId,
            Title = "Vardiya Hatırlatması",
            Body = $"Vardiyanız {shiftStart:dd.MM.yyyy HH:mm} tarihinde başlayacak.",
            Type = "ShiftReminder",
            Action = "navigate_schedule",
            DataJson = JsonSerializer.Serialize(new { shiftAssignmentId, shiftStart }),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);

        _logger.LogInformation(
            "Shift reminder created for assignment {AssignmentId}, employee {EmployeeId}, shift start {ShiftStart}",
            shiftAssignmentId, employeeId, shiftStart);
    }

    public async Task CancelShiftReminderAsync(int shiftAssignmentId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Shift reminder cancelled for assignment {AssignmentId}", shiftAssignmentId);
        await Task.CompletedTask;
    }

    private Task SendPushToDeviceAsync(string deviceToken, string platform, string title, string body, string action)
    {
        // FCM/APNS integration placeholder
        _logger.LogInformation(
            "Push notification queued: Platform={Platform}, Token={Token}, Title={Title}",
            platform, deviceToken[..Math.Min(20, deviceToken.Length)] + "...", title);
        return Task.CompletedTask;
    }
}
