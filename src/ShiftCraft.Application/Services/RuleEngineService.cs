using ShiftCraft.Application.Interfaces;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;

namespace ShiftCraft.Application.Services;

public class RuleEngineService : IRuleEngineService
{
    private readonly IWeeklyScheduleRepository _weeklyScheduleRepository;
    private readonly IWorkRuleRepository _workRuleRepository;
    private readonly ICoreStaffRuleRepository _coreStaffRuleRepository;
    private readonly IRuleViolationRepository _ruleViolationRepository;

    public RuleEngineService(
        IWeeklyScheduleRepository weeklyScheduleRepository,
        IWorkRuleRepository workRuleRepository,
        ICoreStaffRuleRepository coreStaffRuleRepository,
        IRuleViolationRepository ruleViolationRepository)
    {
        _weeklyScheduleRepository = weeklyScheduleRepository;
        _workRuleRepository = workRuleRepository;
        _coreStaffRuleRepository = coreStaffRuleRepository;
        _ruleViolationRepository = ruleViolationRepository;
    }

    public async Task<IEnumerable<RuleViolation>> ValidateScheduleAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        var violations = new List<RuleViolation>();
        violations.AddRange(await ValidateMaxDailyMinutesAsync(weeklyScheduleId, cancellationToken));
        violations.AddRange(await ValidateWeeklyOffDaysAsync(weeklyScheduleId, cancellationToken));
        violations.AddRange(await ValidateCoreStaffMinimumAsync(weeklyScheduleId, cancellationToken));
        return violations;
    }

    public async Task<IEnumerable<RuleViolation>> ValidateMaxDailyMinutesAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        var violations = new List<RuleViolation>();
        var schedule = await _weeklyScheduleRepository.GetByIdWithDaysAsync(weeklyScheduleId, cancellationToken);
        if (schedule == null) return violations;

        var workRule = await _workRuleRepository.GetByBusinessIdAsync(schedule.BusinessId, cancellationToken);
        if (workRule == null) return violations;

        var employeeDailyMinutes = new Dictionary<(int EmployeeId, DateTime Date), int>();

        foreach (var day in schedule.ScheduleDays)
        {
            foreach (var assignment in day.ShiftAssignments)
            {
                var key = (assignment.EmployeeId, day.Date);
                if (!employeeDailyMinutes.ContainsKey(key))
                    employeeDailyMinutes[key] = 0;
                employeeDailyMinutes[key] += assignment.ShiftTemplate.DurationMinutes;
            }
        }


        foreach (var kvp in employeeDailyMinutes)
        {
            if (kvp.Value > workRule.MaxDailyMinutes)
            {
                var violation = new RuleViolation
                {
                    WeeklyScheduleId = weeklyScheduleId,
                    EmployeeId = kvp.Key.EmployeeId,
                    ViolationDate = kvp.Key.Date,
                    RuleCode = RuleCode.MaxDailyMinutes
                };
                violations.Add(violation);
                await _ruleViolationRepository.AddAsync(violation, cancellationToken);
            }
        }

        return violations;
    }

    public async Task<IEnumerable<RuleViolation>> ValidateWeeklyOffDaysAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        var violations = new List<RuleViolation>();
        var schedule = await _weeklyScheduleRepository.GetByIdWithDaysAsync(weeklyScheduleId, cancellationToken);
        if (schedule == null) return violations;

        var workRule = await _workRuleRepository.GetByBusinessIdAsync(schedule.BusinessId, cancellationToken);
        if (workRule == null) return violations;

        var employeeWorkDays = new Dictionary<int, HashSet<DateTime>>();

        foreach (var day in schedule.ScheduleDays)
        {
            foreach (var assignment in day.ShiftAssignments)
            {
                if (!employeeWorkDays.ContainsKey(assignment.EmployeeId))
                    employeeWorkDays[assignment.EmployeeId] = new HashSet<DateTime>();
                employeeWorkDays[assignment.EmployeeId].Add(day.Date);
            }
        }

        var totalDays = schedule.ScheduleDays.Select(d => d.Date).Distinct().Count();

        foreach (var kvp in employeeWorkDays)
        {
            var offDays = totalDays - kvp.Value.Count;
            if (offDays < workRule.MinWeeklyOffDays)
            {
                var violation = new RuleViolation
                {
                    WeeklyScheduleId = weeklyScheduleId,
                    EmployeeId = kvp.Key,
                    ViolationDate = schedule.WeekStartDate,
                    RuleCode = RuleCode.WeeklyOffDays
                };
                violations.Add(violation);
                await _ruleViolationRepository.AddAsync(violation, cancellationToken);
            }
        }

        return violations;
    }

    public async Task<IEnumerable<RuleViolation>> ValidateCoreStaffMinimumAsync(int weeklyScheduleId, CancellationToken cancellationToken = default)
    {
        var violations = new List<RuleViolation>();
        var schedule = await _weeklyScheduleRepository.GetByIdWithDaysAsync(weeklyScheduleId, cancellationToken);
        if (schedule == null) return violations;

        var coreStaffRules = await _coreStaffRuleRepository.GetByBusinessIdAsync(schedule.BusinessId, cancellationToken);
        var rulesByDayType = coreStaffRules.ToDictionary(r => r.DayTypeId, r => r.MinCoreStaffCount);

        foreach (var day in schedule.ScheduleDays)
        {
            if (!rulesByDayType.TryGetValue(day.DayTypeId, out var minCoreStaff))
                continue;

            var coreStaffCount = day.ShiftAssignments.Count(a => a.Employee.IsCoreStaff);

            if (coreStaffCount < minCoreStaff)
            {
                var violation = new RuleViolation
                {
                    WeeklyScheduleId = weeklyScheduleId,
                    EmployeeId = day.ShiftAssignments.FirstOrDefault()?.EmployeeId ?? 0,
                    ViolationDate = day.Date,
                    RuleCode = RuleCode.CoreStaffMinimum
                };
                violations.Add(violation);
                await _ruleViolationRepository.AddAsync(violation, cancellationToken);
            }
        }

        return violations;
    }
}