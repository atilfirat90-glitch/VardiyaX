using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShiftCraft.Domain.Entities;

namespace ShiftCraft.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();

        builder.HasOne(e => e.Business)
            .WithMany(b => b.Employees)
            .HasForeignKey(e => e.BusinessId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}