using ShopFlow.Models.Interfaces;

namespace ShopFlow.Models;

public class ProductVariant : IAuditableEntity, ISoftDeletable
{
    public int VariantID { get; set; }

    public int ProductID { get; set; }

    public string Color { get; set; } = null!;

    public string Size { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public bool IsInStock(int quantity) => StockQuantity >= quantity;

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        if (!IsInStock(quantity))
        {
            throw new InvalidOperationException($"Not enough stock for variant {VariantID}.");
        }

        StockQuantity -= quantity;
    }
}
