using ShopFlow.Models.Interfaces;

namespace ShopFlow.Models;

public class BasketItem : IAuditableEntity
{
    public int BasketItemID { get; set; }

    public int BasketID { get; set; }

    public int VariantID { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Basket Basket { get; set; } = null!;

    public ProductVariant Variant { get; set; } = null!;

    public decimal LineTotal => Variant is null ? 0m : Variant.Price * Quantity;
}
