using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftCraft.Api.Models;
using ShiftCraft.Application.Interfaces;

namespace ShiftCraft.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IShiftAssignmentRepository _shiftAssignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IShiftSwapRepository _swapRepository;
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IWeeklyScheduleRepository _weeklyScheduleRepository;
    private readonly IScheduleDayRepository _scheduleDayRepository;

    public DashboardController(
        IShiftAssignmentRepository shiftAssignmentRepository,
        IEmployeeRepository employeeRepository,
        IShiftSwapRepository swapRepository,
        ITimeOffRepository timeOffRepository,
        IWeeklyScheduleRepository weeklyScheduleRepository,
        IScheduleDayRepository scheduleDayRepository)
    {
        _shiftAssignmentRepository = shiftAssignmentRepository;
        _employeeRepository = employeeRepository;
        _swapRepository = swapRepository;
        _timeOffRepository = timeOffRepository;
        _weeklyScheduleRepository = weeklyScheduleRepository;
        _scheduleDayRepository = scheduleDayRepository;
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(int businessId, CancellationToken ct)
    {
        var employees = await _employeeRepository.GetByBusinessIdAsync(businessId, ct);
        var activeEmployees = employees.Where(e => e.IsActive).ToList();

        var pendingSwaps = await _swapRepository.GetPendingByBusinessAsync(businessId, ct);
        var pendingTimeOff = await _timeOffRepository.GetPendingByBusinessAsync(businessId, ct);

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        if (today.DayOfWeek == DayOfWeek.Sunday)
            weekStart = weekStart.AddDays(-7);
        var weekEnd = weekStart.AddDays(6);

        var todayShiftCount = 0;
        double weeklyHoursTotal = 0;
        var upcomingShifts = new List<ShiftResponseDto>();

        var schedules = await _weeklyScheduleRepository.GetByBusinessIdAsync(businessId, ct);
        foreach (var schedule in schedules)
        {
            var withDays = await _weeklyScheduleRepository.GetByIdWithDaysAsync(schedule.Id, ct);
            if (withDays == null) continue;

            foreach (var day in withDays.ScheduleDays)
            {
                if (day.Date.Date == today)
                {
                    todayShiftCount += day.ShiftAssignments.Count;
                }

                if (day.Date.Date >= weekStart && day.Date.Date <= weekEnd)
                {
                    foreach (var a in day.ShiftAssignments)
                    {
                        if (a.ShiftTemplate != null)
                        {
                            weeklyHoursTotal += a.ShiftTemplate.DurationMinutes / 60.0;
                        }
                    }
                }

                if (day.Date.Date >= today && upcomingShifts.Count < 3)
                {
                    foreach (var a in day.ShiftAssignments.Take(3 - upcomingShifts.Count))
                    {
                        upcomingShifts.Add(new ShiftResponseDto
                        {
                            Id = a.Id,
                            EmployeeId = a.EmployeeId,
                            EmployeeName = a.Employee?.Name ?? string.Empty,
                            Date = day.Date,
                            StartTime = a.ShiftTemplate?.StartTime ?? TimeSpan.Zero,
                            EndTime = a.ShiftTemplate?.EndTime ?? TimeSpan.Zero,
                            DurationMinutes = a.ShiftTemplate?.DurationMinutes ?? 0,
                            Status = withDays.Status.ToString()
                        });
                    }
                }
            }
        }

        var dashboard = new DashboardDto
        {
            TodayShiftCount = todayShiftCount,
            ActiveEmployeeCount = activeEmployees.Count,
            PendingSwapCount = pendingSwaps.Count(),
            PendingTimeOffCount = pendingTimeOff.Count(),
            WeeklyHoursTotal = Math.Round(weeklyHoursTotal, 1),
            UpcomingShifts = upcomingShifts.Take(3).ToList()
        };

        return Ok(dashboard);
    }
}
