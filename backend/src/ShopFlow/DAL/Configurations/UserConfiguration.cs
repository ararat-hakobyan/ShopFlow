using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.UserID);

        builder.Property(user => user.Username)
               .IsRequired()
               .HasMaxLength(50);
        builder.HasIndex(user => user.Username).IsUnique();

        builder.Property(user => user.Email)
               .IsRequired()
               .HasMaxLength(255);
        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.PasswordHash)
               .IsRequired()
               .HasMaxLength(512);

        builder.Property(user => user.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(user => user.UpdatedAt)
               .IsRequired(false);

        builder.HasOne(user => user.Courier)
               .WithOne()
               .HasForeignKey<Courier>(courier => courier.CourierID)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
