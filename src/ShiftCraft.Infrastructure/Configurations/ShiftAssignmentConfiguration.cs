using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class ShiftAssignmentConfiguration : IEntityTypeConfiguration<ShiftAssignment>
{
    public void Configure(EntityTypeBuilder<ShiftAssignment> builder)
    {
        builder.ToTable("ShiftAssignments");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.ScheduleDay)
            .WithMany(d => d.ShiftAssignments)
            .HasForeignKey(s => s.ScheduleDayId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Employee)
            .WithMany(e => e.ShiftAssignments)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.Role)
            .WithMany(r => r.ShiftAssignments)
            .HasForeignKey(s => s.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.ShiftTemplate)
            .WithMany(t => t.ShiftAssignments)
            .HasForeignKey(s => s.ShiftTemplateId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}