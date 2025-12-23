using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class ShiftRequirementConfiguration : IEntityTypeConfiguration<ShiftRequirement>
{
    public void Configure(EntityTypeBuilder<ShiftRequirement> builder)
    {
        builder.ToTable("ShiftRequirements");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Business)
            .WithMany(b => b.ShiftRequirements)
            .HasForeignKey(s => s.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.DayType)
            .WithMany(d => d.ShiftRequirements)
            .HasForeignKey(s => s.DayTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.Role)
            .WithMany(r => r.ShiftRequirements)
            .HasForeignKey(s => s.RoleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.ShiftTemplate)
            .WithMany(t => t.ShiftRequirements)
            .HasForeignKey(s => s.ShiftTemplateId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}