using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class PublishLogConfiguration : IEntityTypeConfiguration<PublishLog>
{
    public void Configure(EntityTypeBuilder<PublishLog> builder)
    {
        builder.ToTable("PublishLogs");
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.PublisherUsername)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(p => p.Timestamp);
        builder.HasIndex(p => p.PublisherUsername);
        builder.HasIndex(p => p.BusinessId);

        builder.HasOne(p => p.WeeklySchedule)
            .WithMany()
            .HasForeignKey(p => p.WeeklyScheduleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.Business)
            .WithMany()
            .HasForeignKey(p => p.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
