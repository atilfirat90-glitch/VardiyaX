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
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        IDeviceRegistrationRepository deviceRepository,
        INotificationPreferenceRepository preferenceRepository,
        ILogger<NotificationController> logger)
    {
        _deviceRepository = deviceRepository;
        _preferenceRepository = preferenceRepository;
        _logger = logger;
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<NotificationPreferencesDto>> GetPreferences(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
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
        var userId = GetCurrentUserId();
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
        var userId = GetCurrentUserId();
        if (userId == null)
            return BadRequest(new { message = "User context required" });

        if (string.IsNullOrEmpty(request.DeviceToken))
            return BadRequest(new { message = "Device token is required" });

        if (string.IsNullOrEmpty(request.Platform))
            return BadRequest(new { message = "Platform is required" });

        // Check if device already registered
        var existing = await _deviceRepository.GetByDeviceTokenAsync(request.DeviceToken, cancellationToken);
        if (existing != null)
        {
            // Update existing registration
            existing.UserId = userId.Value;
            existing.Platform = request.Platform;
            existing.LastActiveAt = DateTime.UtcNow;
            existing.IsActive = true;
            await _deviceRepository.UpdateAsync(existing, cancellationToken);
        }
        else
        {
            // Create new registration
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

    private int? GetCurrentUserId()
    {
        // In a real implementation, this would come from the JWT claims
        // For now, we'll use a placeholder
        var userIdClaim = User.FindFirst("user_id")?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : null;
    }
}
