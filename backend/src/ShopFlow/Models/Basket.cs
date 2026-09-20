using ShopFlow.Models.Interfaces;

namespace ShopFlow.Models;

public class Basket : IAuditableEntity
{
    public int BasketID { get; set; }

    public int UserID { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    public decimal Total => BasketItems.Sum(item => item.LineTotal);
}
