using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(variant => variant.VariantID);

        builder.Property(variant => variant.Color).HasMaxLength(50).IsRequired(false);
        builder.Property(variant => variant.Size).HasMaxLength(20).IsRequired(false);
        builder.Property(variant => variant.Price).HasPrecision(18, 2).IsRequired();
        builder.Property(variant => variant.StockQuantity).HasDefaultValue(0).IsRequired();

        builder.HasOne(variant => variant.Product)
               .WithMany(product => product.ProductVariants)
               .HasForeignKey(variant => variant.ProductID)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(variant => variant.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(variant => variant.UpdatedAt)
               .IsRequired(false);

        builder.Property(variant => variant.IsDeleted)
               .HasDefaultValue(false)
               .IsRequired();

        builder.HasQueryFilter(variant => !variant.IsDeleted && !variant.Product.IsDeleted);
    }
}
