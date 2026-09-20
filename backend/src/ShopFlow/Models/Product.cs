using ShopFlow.Models.Interfaces;

namespace ShopFlow.Models;

public class Product : IAuditableEntity, ISoftDeletable
{
    public int ProductID { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryID { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
