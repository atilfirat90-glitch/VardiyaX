using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Api.Models;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] // Backward compatibility
[Authorize]
public class ShiftAssignmentController : ControllerBase
{
    private readonly IShiftAssignmentRepository _shiftAssignmentRepository;
    private readonly IShiftValidationService _shiftValidationService;
    private readonly IWeeklyScheduleRepository _weeklyScheduleRepository;
    private readonly IScheduleDayRepository _scheduleDayRepository;
    private readonly IShiftTemplateRepository _shiftTemplateRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public ShiftAssignmentController(
        IShiftAssignmentRepository shiftAssignmentRepository,
        IShiftValidationService shiftValidationService,
        IWeeklyScheduleRepository weeklyScheduleRepository,
        IScheduleDayRepository scheduleDayRepository,
        IShiftTemplateRepository shiftTemplateRepository,
        IEmployeeRepository employeeRepository)
    {
        _shiftAssignmentRepository = shiftAssignmentRepository;
        _shiftValidationService = shiftValidationService;
        _weeklyScheduleRepository = weeklyScheduleRepository;
        _scheduleDayRepository = scheduleDayRepository;
        _shiftTemplateRepository = shiftTemplateRepository;
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShiftAssignment>>> GetAll(CancellationToken cancellationToken)
    {
        var assignments = await _shiftAssignmentRepository.GetAllAsync(cancellationToken);
        return Ok(assignments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShiftAssignment>> GetById(int id, CancellationToken cancellationToken)
    {
        var assignment = await _shiftAssignmentRepository.GetByIdAsync(id, cancellationToken);
        if (assignment == null) return NotFound();
        return Ok(assignment);
    }

    [HttpGet("scheduleday/{scheduleDayId}")]
    public async Task<ActionResult<IEnumerable<ShiftAssignment>>> GetByScheduleDayId(int scheduleDayId, CancellationToken cancellationToken)
    {
        var assignments = await _shiftAssignmentRepository.GetByScheduleDayIdAsync(scheduleDayId, cancellationToken);
        return Ok(assignments);
    }

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<IEnumerable<ShiftAssignment>>> GetByEmployeeId(int employeeId, CancellationToken cancellationToken)
    {
        var assignments = await _shiftAssignmentRepository.GetByEmployeeIdAsync(employeeId, cancellationToken);
        return Ok(assignments);
    }

    [HttpPost]
    public async Task<ActionResult<ShiftAssignment>> Create(ShiftAssignment assignment, CancellationToken cancellationToken)
    {
        var created = await _shiftAssignmentRepository.AddAsync(assignment, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ShiftAssignment assignment, CancellationToken cancellationToken)
    {
        if (id != assignment.Id) return BadRequest();
        await _shiftAssignmentRepository.UpdateAsync(assignment, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var assignment = await _shiftAssignmentRepository.GetByIdAsync(id, cancellationToken);
        if (assignment == null) return NotFound();
        await _shiftAssignmentRepository.DeleteAsync(assignment, cancellationToken);
        return NoContent();
    }

    [HttpPost("create")]
    public async Task<ActionResult<ShiftResponseDto>> CreateShift([FromBody] CreateShiftRequest request, CancellationToken cancellationToken)
    {
        var errors = await _shiftValidationService.ValidateShiftAsync(
            request.EmployeeId, request.Date, request.StartTime, request.EndTime,
            cancellationToken: cancellationToken);

        if (errors.Count > 0)
        {
            return BadRequest(new { errors });
        }

        var weekStartDate = GetMondayOfWeek(request.Date);

        var weeklySchedule = await FindOrCreateWeeklyScheduleAsync(
            request.BusinessId, weekStartDate, cancellationToken);

        var scheduleDay = await FindOrCreateScheduleDayAsync(
            weeklySchedule.Id, request.Date, cancellationToken);

        var shiftTemplate = await FindOrCreateShiftTemplateAsync(
            request.BusinessId, request.StartTime, request.EndTime, cancellationToken);

        var roleId = request.RoleId ?? 3;

        var assignment = new ShiftAssignment
        {
            ScheduleDayId = scheduleDay.Id,
            EmployeeId = request.EmployeeId,
            RoleId = roleId,
            ShiftTemplateId = shiftTemplate.Id,
            Source = ShiftSource.Manual
        };

        var created = await _shiftAssignmentRepository.AddAsync(assignment, cancellationToken);

        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);

        var response = new ShiftResponseDto
        {
            Id = created.Id,
            EmployeeId = request.EmployeeId,
            EmployeeName = employee?.Name ?? string.Empty,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            DurationMinutes = shiftTemplate.DurationMinutes,
            Status = weeklySchedule.Status.ToString()
        };

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<List<ShiftResponseDto>>> GetByDateRange(
        [FromQuery] int employeeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var assignments = await _shiftAssignmentRepository
            .GetByEmployeeAndDateRangeAsync(employeeId, startDate, endDate, cancellationToken);

        var result = assignments.Select(a => new ShiftResponseDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee?.Name ?? string.Empty,
            Date = a.ScheduleDay?.Date ?? DateTime.MinValue,
            StartTime = a.ShiftTemplate?.StartTime ?? TimeSpan.Zero,
            EndTime = a.ShiftTemplate?.EndTime ?? TimeSpan.Zero,
            DurationMinutes = a.ShiftTemplate?.DurationMinutes ?? 0,
            Status = a.ScheduleDay?.WeeklySchedule?.Status.ToString() ?? "Draft"
        }).ToList();

        return Ok(result);
    }

    [HttpGet("business/{businessId}/date/{date}")]
    public async Task<ActionResult<List<ShiftResponseDto>>> GetByBusinessAndDate(
        int businessId, DateTime date, CancellationToken cancellationToken)
    {
        var weekStartDate = GetMondayOfWeek(date);

        var schedules = await _weeklyScheduleRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        var weeklySchedule = schedules.FirstOrDefault(s => s.WeekStartDate == weekStartDate);

        if (weeklySchedule == null)
        {
            return Ok(new List<ShiftResponseDto>());
        }

        var scheduleWithDays = await _weeklyScheduleRepository
            .GetByIdWithDaysAsync(weeklySchedule.Id, cancellationToken);

        if (scheduleWithDays == null)
        {
            return Ok(new List<ShiftResponseDto>());
        }

        var scheduleDay = scheduleWithDays.ScheduleDays
            .FirstOrDefault(d => d.Date.Date == date.Date);

        if (scheduleDay == null)
        {
            return Ok(new List<ShiftResponseDto>());
        }

        var result = scheduleDay.ShiftAssignments.Select(a => new ShiftResponseDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee?.Name ?? string.Empty,
            Date = scheduleDay.Date,
            StartTime = a.ShiftTemplate?.StartTime ?? TimeSpan.Zero,
            EndTime = a.ShiftTemplate?.EndTime ?? TimeSpan.Zero,
            DurationMinutes = a.ShiftTemplate?.DurationMinutes ?? 0,
            Status = scheduleWithDays.Status.ToString()
        }).ToList();

        return Ok(result);
    }

    private static DateTime GetMondayOfWeek(DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.Date.AddDays(-diff);
    }

    private static int GetDayTypeId(DateTime date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 2 : 1;
    }

    private async Task<WeeklySchedule> FindOrCreateWeeklyScheduleAsync(
        int businessId, DateTime weekStartDate, CancellationToken cancellationToken)
    {
        var schedules = await _weeklyScheduleRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        var existing = schedules.FirstOrDefault(s => s.WeekStartDate == weekStartDate);

        if (existing != null)
            return existing;

        var newSchedule = new WeeklySchedule
        {
            BusinessId = businessId,
            WeekStartDate = weekStartDate,
            Status = ScheduleStatus.Draft
        };

        return await _weeklyScheduleRepository.AddAsync(newSchedule, cancellationToken);
    }

    private async Task<ScheduleDay> FindOrCreateScheduleDayAsync(
        int weeklyScheduleId, DateTime date, CancellationToken cancellationToken)
    {
        var days = await _scheduleDayRepository.GetByWeeklyScheduleIdAsync(weeklyScheduleId, cancellationToken);
        var existing = days.FirstOrDefault(d => d.Date.Date == date.Date);

        if (existing != null)
            return existing;

        var newDay = new ScheduleDay
        {
            WeeklyScheduleId = weeklyScheduleId,
            Date = date.Date,
            DayTypeId = GetDayTypeId(date)
        };

        return await _scheduleDayRepository.AddAsync(newDay, cancellationToken);
    }

    private async Task<ShiftTemplate> FindOrCreateShiftTemplateAsync(
        int businessId, TimeSpan startTime, TimeSpan endTime, CancellationToken cancellationToken)
    {
        var templates = await _shiftTemplateRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        var existing = templates.FirstOrDefault(t => t.StartTime == startTime && t.EndTime == endTime);

        if (existing != null)
            return existing;

        var durationMinutes = (int)(endTime - startTime).TotalMinutes;

        var newTemplate = new ShiftTemplate
        {
            BusinessId = businessId,
            Name = $"{startTime:hh\\:mm}-{endTime:hh\\:mm}",
            StartTime = startTime,
            EndTime = endTime,
            DurationMinutes = durationMinutes
        };

        return await _shiftTemplateRepository.AddAsync(newTemplate, cancellationToken);
    }
}
