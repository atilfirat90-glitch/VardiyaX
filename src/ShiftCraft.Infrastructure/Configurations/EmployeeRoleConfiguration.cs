using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class EmployeeRoleConfiguration : IEntityTypeConfiguration<EmployeeRole>
{
    public void Configure(EntityTypeBuilder<EmployeeRole> builder)
    {
        builder.ToTable("EmployeeRoles");
        builder.HasKey(er => new { er.EmployeeId, er.RoleId });

        builder.HasOne(er => er.Employee)
            .WithMany(e => e.EmployeeRoles)
            .HasForeignKey(er => er.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(er => er.Role)
            .WithMany(r => r.EmployeeRoles)
            .HasForeignKey(er => er.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}