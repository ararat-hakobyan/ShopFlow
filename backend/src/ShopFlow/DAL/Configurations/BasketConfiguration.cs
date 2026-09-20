using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.ToTable("Baskets");
        builder.HasKey(basket => basket.BasketID);

        builder.HasOne(basket => basket.User)
               .WithOne(user => user.Basket)
               .HasForeignKey<Basket>(basket => basket.UserID)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(basket => basket.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(basket => basket.UpdatedAt)
               .IsRequired(false);

        builder.Ignore(basket => basket.Total);
    }
}
