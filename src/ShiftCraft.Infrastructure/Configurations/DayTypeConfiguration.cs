using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class DayTypeConfiguration : IEntityTypeConfiguration<DayType>
{
    public void Configure(EntityTypeBuilder<DayType> builder)
    {
        builder.ToTable("DayTypes");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Code).IsRequired();
    }
}