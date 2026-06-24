using HRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRM.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.EntityId).IsRequired();
        builder.Property(e => e.Action).IsRequired().HasMaxLength(20);
        builder.Property(e => e.ChangedBy).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ChangedAt).IsRequired();
        builder.Property(e => e.OldValues).IsRequired(false).HasColumnType("nvarchar(max)");
        builder.Property(e => e.NewValues).IsRequired().HasColumnType("nvarchar(max)");
    }
}
