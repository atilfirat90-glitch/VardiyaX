using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore;
using ShiftCraft.Application.Services;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;
using ShiftCraft.Infrastructure.Repositories;

namespace ShiftCraft.Tests.PropertyTests;

/// <summary>
/// Feature: vardiyax-clean-architecture
/// Property 5: MaxDailyMinutes Rule Validation
/// Property 6: MinWeeklyOffDays Rule Validation
/// Property 7: CoreStaffMinimum Rule Validation
/// Validates: Requirements 10.3, 10.4, 11.2, 13.1, 13.2, 13.3
/// </summary>
public class RuleEnginePropertyTests
{
    [Property(MaxTest = 50)]
    public Property MaxDailyMinutes_Violation_Detected_When_Exceeded()
    {
        return Prop.ForAll(
            Gen.Choose(100, 400).ToArbitrary(),
            Gen.Choose(2, 4).ToArbitrary(),
            (shiftDuration, shiftCount) =>
            {
                using var context = TestDbContextFactory.Create();
                var maxDailyMinutes = 480;
                var totalMinutes = shiftDuration * shiftCount;
                var shouldViolate = totalMinutes > maxDailyMinutes;

                var (schedule, workRule) = SetupScheduleWithShifts(context, maxDailyMinutes, 1, shiftDuration, shiftCount);

                var service = CreateRuleEngineService(context);
                var violations = service.ValidateMaxDailyMinutesAsync(schedule.Id).GetAwaiter().GetResult();

                if (shouldViolate)
                    return violations.Any(v => v.RuleCode == RuleCode.MaxDailyMinutes);
                else
                    return !violations.Any(v => v.RuleCode == RuleCode.MaxDailyMinutes);
            });
    }

    [Property(MaxTest = 50)]
    public Property WeeklyOffDays_Violation_Detected_When_Insufficient()
    {
        return Prop.ForAll(
            Gen.Choose(5, 7).ToArbitrary(),
            Gen.Choose(1, 2).ToArbitrary(),
            (workDays, minOffDays) =>
            {
                using var context = TestDbContextFactory.Create();
                var totalDays = 7;
                var actualOffDays = totalDays - workDays;
                var shouldViolate = actualOffDays < minOffDays;

                var (schedule, workRule) = SetupScheduleWithWorkDays(context, minOffDays, workDays);

                var service = CreateRuleEngineService(context);
                var violations = service.ValidateWeeklyOffDaysAsync(schedule.Id).GetAwaiter().GetResult();

                if (shouldViolate)
                    return violations.Any(v => v.RuleCode == RuleCode.WeeklyOffDays);
                else
                    return !violations.Any(v => v.RuleCode == RuleCode.WeeklyOffDays);
            });
    }

    [Property(MaxTest = 50)]
    public Property CoreStaffMinimum_Violation_Detected_When_Insufficient()
    {
        return Prop.ForAll(
            Gen.Choose(1, 3).ToArbitrary(),
            Gen.Choose(0, 2).ToArbitrary(),
            (minCoreStaff, actualCoreStaff) =>
            {
                using var context = TestDbContextFactory.Create();
                var shouldViolate = actualCoreStaff < minCoreStaff;

                var schedule = SetupScheduleWithCoreStaff(context, minCoreStaff, actualCoreStaff);

                var service = CreateRuleEngineService(context);
                var violations = service.ValidateCoreStaffMinimumAsync(schedule.Id).GetAwaiter().GetResult();

                if (shouldViolate)
                    return violations.Any(v => v.RuleCode == RuleCode.CoreStaffMinimum);
                else
                    return !violations.Any(v => v.RuleCode == RuleCode.CoreStaffMinimum);
            });
    }

    [Property(MaxTest = 30)]
    public Property ValidateSchedule_Returns_All_Violation_Types()
    {
        return Prop.ForAll(
            Arb.From<bool>(),
            Arb.From<bool>(),
            Arb.From<bool>(),
            (triggerMaxDaily, triggerWeeklyOff, triggerCoreStaff) =>
            {
                using var context = TestDbContextFactory.Create();
                var schedule = SetupComplexSchedule(context, triggerMaxDaily, triggerWeeklyOff, triggerCoreStaff);

                var service = CreateRuleEngineService(context);
                var violations = service.ValidateScheduleAsync(schedule.Id).GetAwaiter().GetResult().ToList();

                var hasMaxDaily = violations.Any(v => v.RuleCode == RuleCode.MaxDailyMinutes);
                var hasWeeklyOff = violations.Any(v => v.RuleCode == RuleCode.WeeklyOffDays);
                var hasCoreStaff = violations.Any(v => v.RuleCode == RuleCode.CoreStaffMinimum);

                return (triggerMaxDaily == hasMaxDaily || !triggerMaxDaily) &&
                       (triggerWeeklyOff == hasWeeklyOff || !triggerWeeklyOff) &&
                       (triggerCoreStaff == hasCoreStaff || !triggerCoreStaff);
            });
    }

    private static RuleEngineService CreateRuleEngineService(ShiftCraft.Infrastructure.Data.ShiftCraftDbContext context)
    {
        var weeklyScheduleRepo = new WeeklyScheduleRepository(context);
        var workRuleRepo = new WorkRuleRepository(context);
        var coreStaffRuleRepo = new CoreStaffRuleRepository(context);
        var ruleViolationRepo = new RuleViolationRepository(context);
        return new RuleEngineService(weeklyScheduleRepo, workRuleRepo, coreStaffRuleRepo, ruleViolationRepo);
    }

    private static (WeeklySchedule schedule, WorkRule workRule) SetupScheduleWithShifts(
        ShiftCraft.Infrastructure.Data.ShiftCraftDbContext context,
        int maxDailyMinutes, int minOffDays, int shiftDuration, int shiftCount)
    {
        var business = new Business { Name = "Test", Timezone = "UTC" };
        context.Businesses.Add(business);

        var dayType = new DayType { Code = "Weekday" };
        context.DayTypes.Add(dayType);

        var role = new Role { Name = "Worker" };
        context.Roles.Add(role);
        context.SaveChanges();

        var workRule = new WorkRule
        {
            BusinessId = business.Id,
            IsSevenDaysOpen = true,
            MaxDailyMinutes = maxDailyMinutes,
            MinWeeklyOffDays = minOffDays
        };
        context.WorkRules.Add(workRule);

        var employee = new Employee
        {
            BusinessId = business.Id,
            Name = "Test Employee",
            IsCoreStaff = false,
            WeeklyMaxMinutes = 2400,
            IsActive = true
        };
        context.Employees.Add(employee);

        var template = new ShiftTemplate
        {
            BusinessId = business.Id,
            Name = "Shift",
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(9).Add(TimeSpan.FromMinutes(shiftDuration)),
            DurationMinutes = shiftDuration
        };
        context.ShiftTemplates.Add(template);

        var schedule = new WeeklySchedule
        {
            BusinessId = business.Id,
            WeekStartDate = DateTime.UtcNow.Date,
            Status = ScheduleStatus.Draft
        };
        context.WeeklySchedules.Add(schedule);
        context.SaveChanges();

        var day = new ScheduleDay
        {
            WeeklyScheduleId = schedule.Id,
            Date = DateTime.UtcNow.Date,
            DayTypeId = dayType.Id
        };
        context.ScheduleDays.Add(day);
        context.SaveChanges();

        for (int i = 0; i < shiftCount; i++)
        {
            var assignment = new ShiftAssignment
            {
                ScheduleDayId = day.Id,
                EmployeeId = employee.Id,
                RoleId = role.Id,
                ShiftTemplateId = template.Id,
                Source = ShiftSource.Manual
            };
            context.ShiftAssignments.Add(assignment);
        }
        context.SaveChanges();

        return (schedule, workRule);
    }

    private static (WeeklySchedule schedule, WorkRule workRule) SetupScheduleWithWorkDays(
        ShiftCraft.Infrastructure.Data.ShiftCraftDbContext context,
        int minOffDays, int workDays)
    {
        var business = new Business { Name = "Test", Timezone = "UTC" };
        context.Businesses.Add(business);

        var dayType = new DayType { Code = "Weekday" };
        context.DayTypes.Add(dayType);

        var role = new Role { Name = "Worker" };
        context.Roles.Add(role);
        context.SaveChanges();

        var workRule = new WorkRule
        {
            BusinessId = business.Id,
            IsSevenDaysOpen = true,
            MaxDailyMinutes = 480,
            MinWeeklyOffDays = minOffDays
        };
        context.WorkRules.Add(workRule);

        var employee = new Employee
        {
            BusinessId = business.Id,
            Name = "Test Employee",
            IsCoreStaff = false,
            WeeklyMaxMinutes = 2400,
            IsActive = true
        };
        context.Employees.Add(employee);

        var template = new ShiftTemplate
        {
            BusinessId = business.Id,
            Name = "Shift",
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17),
            DurationMinutes = 480
        };
        context.ShiftTemplates.Add(template);

        var weekStart = DateTime.UtcNow.Date;
        var schedule = new WeeklySchedule
        {
            BusinessId = business.Id,
            WeekStartDate = weekStart,
            Status = ScheduleStatus.Draft
        };
        context.WeeklySchedules.Add(schedule);
        context.SaveChanges();

        for (int i = 0; i < 7; i++)
        {
            var day = new ScheduleDay
            {
                WeeklyScheduleId = schedule.Id,
                Date = weekStart.AddDays(i),
                DayTypeId = dayType.Id
            };
            context.ScheduleDays.Add(day);
        }
        context.SaveChanges();

        var days = context.ScheduleDays.Where(d => d.WeeklyScheduleId == schedule.Id).OrderBy(d => d.Date).ToList();
        for (int i = 0; i < workDays && i < days.Count; i++)
        {
            var assignment = new ShiftAssignment
            {
                ScheduleDayId = days[i].Id,
                EmployeeId = employee.Id,
                RoleId = role.Id,
                ShiftTemplateId = template.Id,
                Source = ShiftSource.Manual
            };
            context.ShiftAssignments.Add(assignment);
        }
        context.SaveChanges();

        return (schedule, workRule);
    }

    private static WeeklySchedule SetupScheduleWithCoreStaff(
        ShiftCraft.Infrastructure.Data.ShiftCraftDbContext context,
        int minCoreStaff, int actualCoreStaff)
    {
        var business = new Business { Name = "Test", Timezone = "UTC" };
        context.Businesses.Add(business);

        var dayType = new DayType { Code = "Weekday" };
        context.DayTypes.Add(dayType);

        var role = new Role { Name = "Worker" };
        context.Roles.Add(role);
        context.SaveChanges();

        var coreStaffRule = new CoreStaffRule
        {
            BusinessId = business.Id,
            DayTypeId = dayType.Id,
            MinCoreStaffCount = minCoreStaff
        };
        context.CoreStaffRules.Add(coreStaffRule);

        var workRule = new WorkRule
        {
            BusinessId = business.Id,
            IsSevenDaysOpen = true,
            MaxDailyMinutes = 480,
            MinWeeklyOffDays = 1
        };
        context.WorkRules.Add(workRule);

        var template = new ShiftTemplate
        {
            BusinessId = business.Id,
            Name = "Shift",
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17),
            DurationMinutes = 480
        };
        context.ShiftTemplates.Add(template);

        var schedule = new WeeklySchedule
        {
            BusinessId = business.Id,
            WeekStartDate = DateTime.UtcNow.Date,
            Status = ScheduleStatus.Draft
        };
        context.WeeklySchedules.Add(schedule);
        context.SaveChanges();

        var day = new ScheduleDay
        {
            WeeklyScheduleId = schedule.Id,
            Date = DateTime.UtcNow.Date,
            DayTypeId = dayType.Id
        };
        context.ScheduleDays.Add(day);
        context.SaveChanges();

        for (int i = 0; i < actualCoreStaff; i++)
        {
            var coreEmployee = new Employee
            {
                BusinessId = business.Id,
                Name = $"Core Employee {i}",
                IsCoreStaff = true,
                WeeklyMaxMinutes = 2400,
                IsActive = true
            };
            context.Employees.Add(coreEmployee);
            context.SaveChanges();

            var assignment = new ShiftAssignment
            {
                ScheduleDayId = day.Id,
                EmployeeId = coreEmployee.Id,
                RoleId = role.Id,
                ShiftTemplateId = template.Id,
                Source = ShiftSource.Manual
            };
            context.ShiftAssignments.Add(assignment);
        }
        context.SaveChanges();

        return schedule;
    }

    private static WeeklySchedule SetupComplexSchedule(
        ShiftCraft.Infrastructure.Data.ShiftCraftDbContext context,
        bool triggerMaxDaily, bool triggerWeeklyOff, bool triggerCoreStaff)
    {
        var business = new Business { Name = "Test", Timezone = "UTC" };
        context.Businesses.Add(business);

        var dayType = new DayType { Code = "Weekday" };
        context.DayTypes.Add(dayType);

        var role = new Role { Name = "Worker" };
        context.Roles.Add(role);
        context.SaveChanges();

        var workRule = new WorkRule
        {
            BusinessId = business.Id,
            IsSevenDaysOpen = true,
            MaxDailyMinutes = 480,
            MinWeeklyOffDays = triggerWeeklyOff ? 3 : 0
        };
        context.WorkRules.Add(workRule);

        var coreStaffRule = new CoreStaffRule
        {
            BusinessId = business.Id,
            DayTypeId = dayType.Id,
            MinCoreStaffCount = triggerCoreStaff ? 5 : 0
        };
        context.CoreStaffRules.Add(coreStaffRule);

        var shiftDuration = triggerMaxDaily ? 300 : 240;
        var template = new ShiftTemplate
        {
            BusinessId = business.Id,
            Name = "Shift",
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(9).Add(TimeSpan.FromMinutes(shiftDuration)),
            DurationMinutes = shiftDuration
        };
        context.ShiftTemplates.Add(template);

        var employee = new Employee
        {
            BusinessId = business.Id,
            Name = "Test Employee",
            IsCoreStaff = false,
            WeeklyMaxMinutes = 2400,
            IsActive = true
        };
        context.Employees.Add(employee);

        var weekStart = DateTime.UtcNow.Date;
        var schedule = new WeeklySchedule
        {
            BusinessId = business.Id,
            WeekStartDate = weekStart,
            Status = ScheduleStatus.Draft
        };
        context.WeeklySchedules.Add(schedule);
        context.SaveChanges();

        for (int i = 0; i < 7; i++)
        {
            var day = new ScheduleDay
            {
                WeeklyScheduleId = schedule.Id,
                Date = weekStart.AddDays(i),
                DayTypeId = dayType.Id
            };
            context.ScheduleDays.Add(day);
        }
        context.SaveChanges();

        var days = context.ScheduleDays.Where(d => d.WeeklyScheduleId == schedule.Id).ToList();
        var workDays = triggerWeeklyOff ? 7 : 4;

        for (int i = 0; i < workDays && i < days.Count; i++)
        {
            var shiftCount = triggerMaxDaily ? 2 : 1;
            for (int j = 0; j < shiftCount; j++)
            {
                var assignment = new ShiftAssignment
                {
                    ScheduleDayId = days[i].Id,
                    EmployeeId = employee.Id,
                    RoleId = role.Id,
                    ShiftTemplateId = template.Id,
                    Source = ShiftSource.Manual
                };
                context.ShiftAssignments.Add(assignment);
            }
        }
        context.SaveChanges();

        return schedule;
    }
}
