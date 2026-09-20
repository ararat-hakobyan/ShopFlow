using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.ToTable("BasketItems");
        builder.HasKey(item => item.BasketItemID);

        builder.Property(item => item.Quantity).IsRequired();

        builder.Property(item => item.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(item => item.UpdatedAt)
               .IsRequired(false);

        builder.HasOne(item => item.Basket)
               .WithMany(basket => basket.BasketItems)
               .HasForeignKey(item => item.BasketID)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Variant)
               .WithMany()
               .HasForeignKey(item => item.VariantID)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => new { item.BasketID, item.VariantID }).IsUnique();

        builder.Ignore(item => item.LineTotal);
    }
}
