using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopFlow.Models;

namespace ShopFlow.DAL.Configurations;

public sealed class CourierConfiguration : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> builder)
    {
        builder.ToTable("Couriers");
        builder.HasKey(courier => courier.CourierID);

        builder.Property(courier => courier.CourierID)
               .ValueGeneratedNever();

        builder.Property(courier => courier.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(courier => courier.LastName).HasMaxLength(50).IsRequired();
        builder.Property(courier => courier.Phone).HasMaxLength(20).IsRequired();
        builder.Property(courier => courier.VehicleType).HasMaxLength(30).IsRequired(false);
        builder.Property(courier => courier.IsActive).HasDefaultValue(true);

        builder.Property(courier => courier.CreatedAt)
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
        builder.Property(courier => courier.UpdatedAt)
               .IsRequired(false);

        builder.Ignore(courier => courier.FullName);
    }
}
