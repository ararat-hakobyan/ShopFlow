using ShopFlow.Models.Interfaces;

namespace ShopFlow.Models;

public class OrderDetail : IAuditableEntity
{
    public int OrderDetailID { get; set; }

    public int OrderID { get; set; }

    public int VariantID { get; set; }

    public int Quantity { get; set; }

    public decimal PriceAtPurchase { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Order Order { get; set; } = null!;

    public ProductVariant Variant { get; set; } = null!;

    public decimal LineTotal => PriceAtPurchase * Quantity;
}
