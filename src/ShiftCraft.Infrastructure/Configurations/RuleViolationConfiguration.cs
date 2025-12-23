using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class RuleViolationConfiguration : IEntityTypeConfiguration<RuleViolation>
{
    public void Configure(EntityTypeBuilder<RuleViolation> builder)
    {
        builder.ToTable("RuleViolations");
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.WeeklySchedule)
            .WithMany(w => w.RuleViolations)
            .HasForeignKey(r => r.WeeklyScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Employee)
            .WithMany(e => e.RuleViolations)
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}