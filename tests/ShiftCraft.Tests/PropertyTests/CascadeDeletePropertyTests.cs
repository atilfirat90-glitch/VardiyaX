using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Domain.Enums;

namespace ShiftCraft.Tests.PropertyTests;

/// <summary>
/// Feature: vardiyax-clean-architecture, Property 2: Cascade Delete Consistency
/// Validates: Requirements 3.3, 6.4, 7.3, 12.5
/// </summary>
public class CascadeDeletePropertyTests
{
    [Property(MaxTest = 100)]
    public Property WeeklySchedule_Cascade_Deletes_ScheduleDays()
    {
        return Prop.ForAll(
            Arb.From<PositiveInt>(),
            (dayCount) =>
            {
                using var context = TestDbContextFactory.Create();
                var business = new Business { Name = "Test", Timezone = "UTC" };
                context.Businesses.Add(business);
                context.SaveChanges();

                var dayType = new DayType { Code = "Weekday" };
                context.DayTypes.Add(dayType);
                context.SaveChanges();

                var schedule = new WeeklySchedule
                {
                    BusinessId = business.Id,
                    WeekStartDate = DateTime.UtcNow.Date,
                    Status = ScheduleStatus.Draft
                };
                context.WeeklySchedules.Add(schedule);
                context.SaveChanges();

                var numDays = Math.Min(dayCount.Get, 7);
                for (int i = 0; i < numDays; i++)
                {
                    context.ScheduleDays.Add(new ScheduleDay
                    {
                        WeeklyScheduleId = schedule.Id,
                        Date = DateTime.UtcNow.Date.AddDays(i),
                        DayTypeId = dayType.Id
                    });
                }
                context.SaveChanges();

                var scheduleId = schedule.Id;
                context.WeeklySchedules.Remove(schedule);
                context.SaveChanges();

                var remainingDays = context.ScheduleDays.Count(d => d.WeeklyScheduleId == scheduleId);
                return remainingDays == 0;
            });
    }

    [Property(MaxTest = 100)]
    public Property ScheduleDay_Cascade_Deletes_ShiftAssignments()
    {
        return Prop.ForAll(
            Arb.From<PositiveInt>(),
            (assignmentCount) =>
            {
                using var context = TestDbContextFactory.Create();
                
                var business = new Business { Name = "Test", Timezone = "UTC" };
                context.Businesses.Add(business);
                
                var dayType = new DayType { Code = "Weekday" };
                context.DayTypes.Add(dayType);
                
                var role = new Role { Name = "Worker" };
                context.Roles.Add(role);
                context.SaveChanges();

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
                    Name = "Morning",
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

                var numAssignments = Math.Min(assignmentCount.Get, 5);
                for (int i = 0; i < numAssignments; i++)
                {
                    context.ShiftAssignments.Add(new ShiftAssignment
                    {
                        ScheduleDayId = day.Id,
                        EmployeeId = employee.Id,
                        RoleId = role.Id,
                        ShiftTemplateId = template.Id,
                        Source = ShiftSource.Manual
                    });
                }
                context.SaveChanges();

                var dayId = day.Id;
                context.ScheduleDays.Remove(day);
                context.SaveChanges();

                var remainingAssignments = context.ShiftAssignments.Count(a => a.ScheduleDayId == dayId);
                return remainingAssignments == 0;
            });
    }

    [Property(MaxTest = 100)]
    public Property Employee_Cascade_Deletes_EmployeeRoles()
    {
        return Prop.ForAll(
            Arb.From<PositiveInt>(),
            (roleCount) =>
            {
                using var context = TestDbContextFactory.Create();
                
                var business = new Business { Name = "Test", Timezone = "UTC" };
                context.Businesses.Add(business);
                context.SaveChanges();

                var employee = new Employee
                {
                    BusinessId = business.Id,
                    Name = "Test Employee",
                    IsCoreStaff = false,
                    WeeklyMaxMinutes = 2400,
                    IsActive = true
                };
                context.Employees.Add(employee);
                context.SaveChanges();

                var numRoles = Math.Min(roleCount.Get, 5);
                for (int i = 0; i < numRoles; i++)
                {
                    var role = new Role { Name = $"Role{i}" };
                    context.Roles.Add(role);
                    context.SaveChanges();

                    context.EmployeeRoles.Add(new EmployeeRole
                    {
                        EmployeeId = employee.Id,
                        RoleId = role.Id
                    });
                }
                context.SaveChanges();

                var employeeId = employee.Id;
                context.Employees.Remove(employee);
                context.SaveChanges();

                var remainingRoles = context.EmployeeRoles.Count(er => er.EmployeeId == employeeId);
                return remainingRoles == 0;
            });
    }
}