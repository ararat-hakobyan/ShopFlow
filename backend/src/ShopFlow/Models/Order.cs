using ShopFlow.Models.Interfaces;
using ShopFlow.Enums;

namespace ShopFlow.Models;

public class Order : IAuditableEntity
{
    public int OrderID { get; set; }

    public int UserID { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public int? CourierID { get; set; }

    public DateTime? EstimatedDeliveryTime { get; set; }

    public string? CourierCoordinates { get; set; }

    public string? ProofOfDeliveryUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    public Courier? Courier { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public bool IsAssigned => CourierID.HasValue;

    public void RecalculateTotal()
    {
        TotalAmount = OrderDetails.Sum(detail => detail.LineTotal);
    }

    public void AssignTo(int courierId)
    {
        if (IsAssigned)
        {
            throw new InvalidOperationException($"Order {OrderID} is already assigned to a courier.");
        }

        CourierID = courierId;
        Status = OrderStatus.OutForDelivery;
    }

    public void MarkAsDelivered()
    {
        Status = OrderStatus.Delivered;
    }
}
