using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class LoginLogConfiguration : IEntityTypeConfiguration<LoginLog>
{
    public void Configure(EntityTypeBuilder<LoginLog> builder)
    {
        builder.ToTable("LoginLogs");
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Username)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(l => l.Action)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(l => l.DeviceInfo)
            .HasMaxLength(500);
        
        builder.Property(l => l.FailureReason)
            .HasMaxLength(500);
        
        builder.Property(l => l.IpAddress)
            .HasMaxLength(50);
        
        builder.HasIndex(l => l.Timestamp);
        builder.HasIndex(l => l.Username);
        builder.HasIndex(l => l.BusinessId);

        builder.HasOne(l => l.Business)
            .WithMany()
            .HasForeignKey(l => l.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
