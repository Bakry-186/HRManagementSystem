using HRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRM.Infrastructure.Persistence.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.EmployeeId).IsRequired();
        builder.Property(ar => ar.Date).IsRequired();
        builder.Property(ar => ar.CheckInTime).IsRequired(false);
        builder.Property(ar => ar.CheckOutTime).IsRequired(false);
        builder.Property(ar => ar.Status).IsRequired().HasMaxLength(20);
        builder.Property(ar => ar.Notes).HasMaxLength(255);

        builder.Property(ar => ar.CreatedAt).IsRequired();
        builder.Property(ar => ar.UpdatedAt).IsRequired();
        builder.Property(ar => ar.IsActive).HasDefaultValueSql("1");

        builder.HasIndex(ar => new { ar.EmployeeId, ar.Date })
            .IsUnique()
            .HasFilter($"[{nameof(AttendanceRecord.IsActive)}] = 1");

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(ar => ar.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
