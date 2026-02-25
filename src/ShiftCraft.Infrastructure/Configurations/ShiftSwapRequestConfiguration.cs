using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class ShiftSwapRequestConfiguration : IEntityTypeConfiguration<ShiftSwapRequest>
{
    public void Configure(EntityTypeBuilder<ShiftSwapRequest> builder)
    {
        builder.ToTable("ShiftSwapRequests");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Status).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Reason).HasMaxLength(500);
        builder.Property(s => s.ResolvedBy).HasMaxLength(100);
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasOne(s => s.Requester)
            .WithMany()
            .HasForeignKey(s => s.RequesterId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.RequesterShift)
            .WithMany()
            .HasForeignKey(s => s.RequesterShiftId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.TargetEmployee)
            .WithMany()
            .HasForeignKey(s => s.TargetEmployeeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.TargetShift)
            .WithMany()
            .HasForeignKey(s => s.TargetShiftId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
