using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(category => category.CategoryID);

        builder.Property(category => category.Name)
               .IsRequired()
               .HasMaxLength(100);
        builder.HasIndex(category => category.Name).IsUnique();

        builder.HasMany(category => category.Products)
               .WithOne(product => product.Category)
               .HasForeignKey(product => product.CategoryID);

        builder.Property(category => category.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(category => category.UpdatedAt)
               .IsRequired(false);
    }
}
