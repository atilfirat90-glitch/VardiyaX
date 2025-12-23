using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class WeeklyScheduleConfiguration : IEntityTypeConfiguration<WeeklySchedule>
{
    public void Configure(EntityTypeBuilder<WeeklySchedule> builder)
    {
        builder.ToTable("WeeklySchedules");
        builder.HasKey(w => w.Id);

        builder.HasOne(w => w.Business)
            .WithMany(b => b.WeeklySchedules)
            .HasForeignKey(w => w.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}