using HRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRM.Infrastructure.Persistence.Configurations;

public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
{
    public void Configure(EntityTypeBuilder<PayrollRecord> builder)
    {
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.PeriodStart).IsRequired();
        builder.Property(pr => pr.PeriodEnd).IsRequired();
        builder.Property(pr => pr.BasicSalary).IsRequired().HasPrecision(18, 2);
        builder.Property(pr => pr.OvertimeAmount).IsRequired().HasPrecision(18, 2);
        builder.Property(pr => pr.Bonus).IsRequired().HasPrecision(18, 2);
        builder.Property(pr => pr.Deductions).IsRequired().HasPrecision(18, 2);
        builder.Property(pr => pr.NetPay).IsRequired().HasPrecision(18, 2);
        builder.Property(pr => pr.Status).IsRequired().HasMaxLength(20);

        builder.Property(pr => pr.CreatedAt).IsRequired();
        builder.Property(pr => pr.UpdatedAt).IsRequired();
        builder.Property(pr => pr.IsActive).HasDefaultValue(true);

        builder.HasIndex(pr => new { pr.EmployeeId, pr.PeriodStart })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.HasOne<Employee>()
            .WithMany()
            .HasForeignKey(pr => pr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
