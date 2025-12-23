using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class DeviceRegistrationConfiguration : IEntityTypeConfiguration<DeviceRegistration>
{
    public void Configure(EntityTypeBuilder<DeviceRegistration> builder)
    {
        builder.ToTable("DeviceRegistrations");
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.DeviceToken)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(d => d.Platform)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => d.DeviceToken);

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
