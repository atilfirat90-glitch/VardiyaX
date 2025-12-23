using FsCheck;
using FsCheck.Xunit;
using Microsoft.EntityFrameworkCore;
using ShiftCraft.Domain.Entities;
using ShiftCraft.Infrastructure.Data;
using ShiftCraft.Infrastructure.Repositories;

namespace ShiftCraft.Tests.PropertyTests;

/// <summary>
/// Feature: vardiyax-clean-architecture, Property 1: Entity Association Validation
/// Validates: Requirements 2.1, 5.1, 6.1, 7.1, 8.1, 9.1, 10.1, 11.1, 12.2
/// </summary>
public class EntityAssociationPropertyTests
{
    [Property(MaxTest = 100)]
    public Property Employee_Requires_Valid_Business_Association()
    {
        return Prop.ForAll(
            Arb.From<NonEmptyString>(),
            (employeeName) =>
            {
                using var context = TestDbContextFactory.Create();
                var business = new Business { Name = "Test Business", Timezone = "UTC" };
                context.Businesses.Add(business);
                context.SaveChanges();

                var employee = new Employee
                {
                    BusinessId = business.Id,
                    Name = employeeName.Get,
                    IsCoreStaff = false,
                    WeeklyMaxMinutes = 2400,
                    IsActive = true
                };
                context.Employees.Add(employee);
                context.SaveChanges();

                var savedEmployee = context.Employees.Include(e => e.Business).First(e => e.Id == employee.Id);
                return savedEmployee.Business != null && savedEmployee.BusinessId == business.Id;
            });
    }

    [Property(MaxTest = 100)]
    public Property ShiftTemplate_Requires_Valid_Business_Association()
    {
        return Prop.ForAll(
            Arb.From<NonEmptyString>(),
            (templateName) =>
            {
                using var context = TestDbContextFactory.Create();
                var business = new Business { Name = "Test Business", Timezone = "UTC" };
                context.Businesses.Add(business);
                context.SaveChanges();

                var template = new ShiftTemplate
                {
                    BusinessId = business.Id,
                    Name = templateName.Get,
                    StartTime = TimeSpan.FromHours(9),
                    EndTime = TimeSpan.FromHours(17),
                    DurationMinutes = 480
                };
                context.ShiftTemplates.Add(template);
                context.SaveChanges();

                var saved = context.ShiftTemplates.Include(t => t.Business).First(t => t.Id == template.Id);
                return saved.Business != null && saved.BusinessId == business.Id;
            });
    }

    [Property(MaxTest = 100)]
    public Property WeeklySchedule_Requires_Valid_Business_Association()
    {
        return Prop.ForAll(
            Arb.From<DateTime>(),
            (weekStart) =>
            {
                using var context = TestDbContextFactory.Create();
                var business = new Business { Name = "Test Business", Timezone = "UTC" };
                context.Businesses.Add(business);
                context.SaveChanges();

                var schedule = new WeeklySchedule
                {
                    BusinessId = business.Id,
                    WeekStartDate = weekStart.Date,
                    Status = Domain.Enums.ScheduleStatus.Draft
                };
                context.WeeklySchedules.Add(schedule);
                context.SaveChanges();

                var saved = context.WeeklySchedules.Include(s => s.Business).First(s => s.Id == schedule.Id);
                return saved.Business != null && saved.BusinessId == business.Id;
            });
    }
}