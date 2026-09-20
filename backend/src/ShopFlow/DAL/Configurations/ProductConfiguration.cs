using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(product => product.ProductID);

        builder.Property(product => product.ProductName)
               .HasMaxLength(150)
               .IsRequired();
        builder.HasIndex(product => product.ProductName);

        builder.HasOne(product => product.Category)
               .WithMany(category => category.Products)
               .HasForeignKey(product => product.CategoryID);

        builder.Property(product => product.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(product => product.UpdatedAt)
               .IsRequired(false);

        builder.Property(product => product.IsDeleted)
               .HasDefaultValue(false)
               .IsRequired();

        builder.HasQueryFilter(product => !product.IsDeleted);
    }
}
