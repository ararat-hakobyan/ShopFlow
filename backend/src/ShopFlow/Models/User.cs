using ShopFlow.Models.Interfaces;
using ShopFlow.Enums;

namespace ShopFlow.Models;

public class User : IAuditableEntity
{
    public int UserID { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Basket? Basket { get; set; }

    public Courier? Courier { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
