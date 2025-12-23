using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class ScheduleDayConfiguration : IEntityTypeConfiguration<ScheduleDay>
{
    public void Configure(EntityTypeBuilder<ScheduleDay> builder)
    {
        builder.ToTable("ScheduleDays");
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.WeeklySchedule)
            .WithMany(w => w.ScheduleDays)
            .HasForeignKey(s => s.WeeklyScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.DayType)
            .WithMany(d => d.ScheduleDays)
            .HasForeignKey(s => s.DayTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}