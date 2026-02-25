using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class TeamMessageConfiguration : IEntityTypeConfiguration<TeamMessage>
{
    public void Configure(EntityTypeBuilder<TeamMessage> builder)
    {
        builder.ToTable("TeamMessages");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);
        builder.Property(m => m.Channel).IsRequired().HasMaxLength(50);
        builder.Property(m => m.CreatedAt).IsRequired();

        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
