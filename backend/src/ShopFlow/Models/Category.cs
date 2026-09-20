using ShopFlow.Models.Interfaces;

namespace ShopFlow.Models;

public class Category : IAuditableEntity
{
    public int CategoryID { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
