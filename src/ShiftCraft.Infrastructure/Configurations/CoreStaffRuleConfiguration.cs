using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class CoreStaffRuleConfiguration : IEntityTypeConfiguration<CoreStaffRule>
{
    public void Configure(EntityTypeBuilder<CoreStaffRule> builder)
    {
        builder.ToTable("CoreStaffRules");
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Business)
            .WithMany(b => b.CoreStaffRules)
            .HasForeignKey(c => c.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.DayType)
            .WithMany(d => d.CoreStaffRules)
            .HasForeignKey(c => c.DayTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}