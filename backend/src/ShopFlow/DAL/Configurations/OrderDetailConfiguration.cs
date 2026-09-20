using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");
        builder.HasKey(detail => detail.OrderDetailID);

        builder.Property(detail => detail.Quantity).IsRequired();
        builder.Property(detail => detail.PriceAtPurchase).HasPrecision(18, 2).IsRequired();

        builder.HasOne(detail => detail.Order)
               .WithMany(order => order.OrderDetails)
               .HasForeignKey(detail => detail.OrderID)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(detail => detail.Variant)
               .WithMany(variant => variant.OrderDetails)
               .HasForeignKey(detail => detail.VariantID)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(detail => new { detail.OrderID, detail.VariantID }).IsUnique();

        builder.Property(detail => detail.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(detail => detail.UpdatedAt)
               .IsRequired(false);

        builder.Ignore(detail => detail.LineTotal);
    }
}
