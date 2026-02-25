using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Api.Models;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using System.Security.Claims;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly IDeviceRegistrationRepository _deviceRepository;
    private readonly INotificationPreferenceRepository _preferenceRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        IDeviceRegistrationRepository deviceRepository,
        INotificationPreferenceRepository preferenceRepository,
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ILogger<NotificationController> logger)
    {
        _deviceRepository = deviceRepository;
        _preferenceRepository = preferenceRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        var notifications = await _notificationRepository.GetByUserIdAsync(userId.Value, page, pageSize, cancellationToken);
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId.Value, cancellationToken);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Body = n.Body,
            Type = n.Type,
            Action = n.Action,
            DataJson = n.DataJson,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            ReadAt = n.ReadAt
        });

        return Ok(new NotificationListResponse
        {
            Notifications = dtos,
            UnreadCount = unreadCount,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("unread")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotifications(CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId.Value, cancellationToken);
        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Body = n.Body,
            Type = n.Type,
            Action = n.Action,
            DataJson = n.DataJson,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        });

        return Ok(dtos);
    }

    [HttpGet("unread/count")]
    public async Task<ActionResult<object>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        var count = await _notificationRepository.GetUnreadCountAsync(userId.Value, cancellationToken);
        return Ok(new { count });
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification == null)
            return NotFound();

        if (notification.UserId != userId.Value)
            return Forbid();

        await _notificationRepository.MarkAsReadAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        await _notificationRepository.MarkAllAsReadAsync(userId.Value, cancellationToken);
        _logger.LogInformation("All notifications marked as read for user {UserId}", userId);
        return NoContent();
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<NotificationPreferencesDto>> GetPreferences(CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        var preference = await _preferenceRepository.GetOrCreateByUserIdAsync(userId.Value, cancellationToken);

        return Ok(new NotificationPreferencesDto
        {
            ScheduleNotificationsEnabled = preference.ScheduleNotificationsEnabled,
            ViolationNotificationsEnabled = preference.ViolationNotificationsEnabled,
            ShiftRemindersEnabled = preference.ShiftRemindersEnabled,
            ReminderHoursBefore = preference.ReminderHoursBefore
        });
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] NotificationPreferencesDto request, 
        CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        var preference = await _preferenceRepository.GetOrCreateByUserIdAsync(userId.Value, cancellationToken);

        preference.ScheduleNotificationsEnabled = request.ScheduleNotificationsEnabled;
        preference.ViolationNotificationsEnabled = request.ViolationNotificationsEnabled;
        preference.ShiftRemindersEnabled = request.ShiftRemindersEnabled;
        preference.ReminderHoursBefore = request.ReminderHoursBefore;

        await _preferenceRepository.UpdateAsync(preference, cancellationToken);
        _logger.LogInformation("Notification preferences updated for user {UserId}", userId);

        return NoContent();
    }

    [HttpPost("device")]
    public async Task<IActionResult> RegisterDevice(
        [FromBody] RegisterDeviceRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = await GetCurrentUserIdAsync(cancellationToken);
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        if (string.IsNullOrEmpty(request.DeviceToken))
            return BadRequest(new { message = "Device token is required" });

        if (string.IsNullOrEmpty(request.Platform))
            return BadRequest(new { message = "Platform is required" });

        var existing = await _deviceRepository.GetByDeviceTokenAsync(request.DeviceToken, cancellationToken);
        if (existing != null)
        {
            existing.UserId = userId.Value;
            existing.Platform = request.Platform;
            existing.LastActiveAt = DateTime.UtcNow;
            existing.IsActive = true;
            await _deviceRepository.UpdateAsync(existing, cancellationToken);
        }
        else
        {
            var device = new DeviceRegistration
            {
                UserId = userId.Value,
                DeviceToken = request.DeviceToken,
                Platform = request.Platform,
                RegisteredAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow,
                IsActive = true
            };
            await _deviceRepository.AddAsync(device, cancellationToken);
        }

        _logger.LogInformation("Device registered for user {UserId}, platform {Platform}", userId, request.Platform);
        return Ok(new { message = "Device registered successfully" });
    }

    [HttpDelete("device")]
    public async Task<IActionResult> UnregisterDevice(
        [FromQuery] string deviceToken, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(deviceToken))
            return BadRequest(new { message = "Device token is required" });

        var device = await _deviceRepository.GetByDeviceTokenAsync(deviceToken, cancellationToken);
        if (device != null)
        {
            device.IsActive = false;
            await _deviceRepository.UpdateAsync(device, cancellationToken);
            _logger.LogInformation("Device unregistered: {Token}", deviceToken[..Math.Min(20, deviceToken.Length)]);
        }

        return NoContent();
    }

    private async Task<int?> GetCurrentUserIdAsync(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        if (int.TryParse(userIdClaim, out var id))
            return id;

        var username = User.Identity?.Name;
        if (!string.IsNullOrEmpty(username))
        {
            var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (user != null)
                return user.Id;
        }

        return null;
    }
}
