using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class WorkRuleConfiguration : IEntityTypeConfiguration<WorkRule>
{
    public void Configure(EntityTypeBuilder<WorkRule> builder)
    {
        builder.ToTable("WorkRules");
        builder.HasKey(w => w.Id);

        builder.HasOne(w => w.Business)
            .WithMany(b => b.WorkRules)
            .HasForeignKey(w => w.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}