using Microsoft.EntityFrameworkCore;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Data;

public class ShiftCraftDbContext : DbContext
{
    public ShiftCraftDbContext(DbContextOptions<ShiftCraftDbContext> options) : base(options)
    {
    }

    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<EmployeeRole> EmployeeRoles => Set<EmployeeRole>();
    public DbSet<DayType> DayTypes => Set<DayType>();
    public DbSet<ShiftTemplate> ShiftTemplates => Set<ShiftTemplate>();
    public DbSet<WeeklySchedule> WeeklySchedules => Set<WeeklySchedule>();
    public DbSet<ScheduleDay> ScheduleDays => Set<ScheduleDay>();
    public DbSet<ShiftAssignment> ShiftAssignments => Set<ShiftAssignment>();
    public DbSet<ShiftRequirement> ShiftRequirements => Set<ShiftRequirement>();
    public DbSet<WorkRule> WorkRules => Set<WorkRule>();
    public DbSet<CoreStaffRule> CoreStaffRules => Set<CoreStaffRule>();
    public DbSet<RuleViolation> RuleViolations => Set<RuleViolation>();
    public DbSet<User> Users => Set<User>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();
    public DbSet<PublishLog> PublishLogs => Set<PublishLog>();
    public DbSet<DeviceRegistration> DeviceRegistrations => Set<DeviceRegistration>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShiftCraftDbContext).Assembly);

        // Seed Data: DayTypes
        modelBuilder.Entity<DayType>().HasData(
            new DayType { Id = 1, Code = "Weekday" },
            new DayType { Id = 2, Code = "Weekend" },
            new DayType { Id = 3, Code = "Holiday" }
        );

        // Seed Data: Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Manager" },
            new Role { Id = 2, Name = "Supervisor" },
            new Role { Id = 3, Name = "Worker" },
            new Role { Id = 4, Name = "Trainee" }
        );
    }
}