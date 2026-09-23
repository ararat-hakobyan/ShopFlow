using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(order => order.OrderID);

        builder.Property(order => order.TotalAmount).HasPrecision(18, 2);

        builder.Property(order => order.RowVersion).IsRowVersion();

        builder.HasOne(order => order.User)
               .WithMany(user => user.Orders)
               .HasForeignKey(order => order.UserID)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.Courier)
               .WithMany(courier => courier.Orders)
               .HasForeignKey(order => order.CourierID)
               .OnDelete(DeleteBehavior.SetNull);

        builder.Property(order => order.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(order => order.UpdatedAt)
               .IsRequired(false);

        builder.Ignore(order => order.IsAssigned);
    }
}
