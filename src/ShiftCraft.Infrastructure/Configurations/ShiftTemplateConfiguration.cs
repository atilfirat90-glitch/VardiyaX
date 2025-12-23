using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class ShiftTemplateConfiguration : IEntityTypeConfiguration<ShiftTemplate>
{
    public void Configure(EntityTypeBuilder<ShiftTemplate> builder)
    {
        builder.ToTable("ShiftTemplates");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired();

        builder.HasOne(s => s.Business)
            .WithMany(b => b.ShiftTemplates)
            .HasForeignKey(s => s.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}