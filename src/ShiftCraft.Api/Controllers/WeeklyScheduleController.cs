using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;
using System.Security.Claims;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] // Backward compatibility
[Authorize]
public class WeeklyScheduleController : ControllerBase
{
    private readonly IWeeklyScheduleRepository _weeklyScheduleRepository;
    private readonly IScheduleValidationService _scheduleValidationService;
    private readonly IPublishLogRepository _publishLogRepository;
    private readonly IShiftAssignmentRepository _shiftAssignmentRepository;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger<WeeklyScheduleController> _logger;

    public WeeklyScheduleController(
        IWeeklyScheduleRepository weeklyScheduleRepository,
        IScheduleValidationService scheduleValidationService,
        IPublishLogRepository publishLogRepository,
        IShiftAssignmentRepository shiftAssignmentRepository,
        IPushNotificationService pushNotificationService,
        ILogger<WeeklyScheduleController> logger)
    {
        _weeklyScheduleRepository = weeklyScheduleRepository;
        _scheduleValidationService = scheduleValidationService;
        _publishLogRepository = publishLogRepository;
        _shiftAssignmentRepository = shiftAssignmentRepository;
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeeklySchedule>>> GetAll(CancellationToken cancellationToken)
    {
        var schedules = await _weeklyScheduleRepository.GetAllAsync(cancellationToken);
        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WeeklySchedule>> GetById(int id, CancellationToken cancellationToken)
    {
        var schedule = await _weeklyScheduleRepository.GetByIdWithDaysAsync(id, cancellationToken);
        if (schedule == null) return NotFound();
        return Ok(schedule);
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<IEnumerable<WeeklySchedule>>> GetByBusinessId(int businessId, CancellationToken cancellationToken)
    {
        var schedules = await _weeklyScheduleRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        return Ok(schedules);
    }

    [HttpPost]
    public async Task<ActionResult<WeeklySchedule>> Create(WeeklySchedule schedule, CancellationToken cancellationToken)
    {
        schedule.Status = ScheduleStatus.Draft;
        var created = await _weeklyScheduleRepository.AddAsync(schedule, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, WeeklySchedule schedule, CancellationToken cancellationToken)
    {
        if (id != schedule.Id) return BadRequest();
        await _weeklyScheduleRepository.UpdateAsync(schedule, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/publish")]
    public async Task<ActionResult<IEnumerable<RuleViolation>>> Publish(int id, CancellationToken cancellationToken)
    {
        var schedule = await _weeklyScheduleRepository.GetByIdAsync(id, cancellationToken);
        if (schedule == null) return NotFound();

        var violations = await _scheduleValidationService.ValidateOnPublishAsync(id, cancellationToken);
        
        schedule.Status = ScheduleStatus.Published;
        await _weeklyScheduleRepository.UpdateAsync(schedule, cancellationToken);

        // Log the publish action and get affected employees
        var affectedEmployeeIds = await LogPublishAction(schedule, cancellationToken);

        // Send push notifications to affected employees
        if (affectedEmployeeIds.Any())
        {
            await _pushNotificationService.SendSchedulePublishedNotificationAsync(
                schedule.Id, affectedEmployeeIds.ToList(), cancellationToken);
        }

        _logger.LogInformation("Schedule {ScheduleId} published by {User}, notified {Count} employees", 
            id, User.Identity?.Name, affectedEmployeeIds.Count());

        return Ok(violations);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var schedule = await _weeklyScheduleRepository.GetByIdAsync(id, cancellationToken);
        if (schedule == null) return NotFound();
        await _weeklyScheduleRepository.DeleteAsync(schedule, cancellationToken);
        return NoContent();
    }

    private async Task<IEnumerable<int>> LogPublishAction(WeeklySchedule schedule, CancellationToken cancellationToken)
    {
        // Get affected employees
        var assignments = await _shiftAssignmentRepository.GetByWeeklyScheduleIdAsync(schedule.Id, cancellationToken);
        var affectedEmployeeIds = assignments.Select(a => a.EmployeeId).Distinct().ToList();

        var publishLog = new PublishLog
        {
            WeeklyScheduleId = schedule.Id,
            PublisherUsername = User.Identity?.Name ?? "unknown",
            Timestamp = DateTime.UtcNow,
            AffectedEmployeeCount = affectedEmployeeIds.Count,
            BusinessId = schedule.BusinessId
        };

        await _publishLogRepository.AddAsync(publishLog, cancellationToken);
        
        return affectedEmployeeIds;
    }
}