using HRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace HRM.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(60);
        builder.Property(u => u.Role).IsRequired().HasMaxLength(50);

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();
        builder.Property(u => u.IsActive).HasDefaultValueSql("1");

        builder.HasIndex(u => u.Username).IsUnique();
    }
}
