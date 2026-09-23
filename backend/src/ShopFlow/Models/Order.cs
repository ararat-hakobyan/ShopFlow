using ShopFlow.Models.Interfaces;
using ShopFlow.Enums;

namespace ShopFlow.Models;

public class Order : IAuditableEntity
{
    public int OrderID { get; set; }

    public int UserID { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public int? CourierID { get; private set; }

    public DateTime? EstimatedDeliveryTime { get; set; }

    public string? CourierCoordinates { get; set; }

    public string? ProofOfDeliveryUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public User User { get; set; } = null!;

    public Courier? Courier { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public bool IsAssigned => CourierID.HasValue;

    public bool CanChangeTo(OrderStatus newStatus) => (Status, newStatus) switch
    {
        (OrderStatus.Pending, OrderStatus.OutForDelivery) => true,
        (OrderStatus.Pending, OrderStatus.Rejected) => true,
        (OrderStatus.OutForDelivery, OrderStatus.Pending) => true,
        (OrderStatus.OutForDelivery, OrderStatus.Delivered) => true,
        (OrderStatus.OutForDelivery, OrderStatus.Rejected) => true,
        _ => false
    };

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

    public void ReturnToPending()
    {
        CourierID = null;
        Status = OrderStatus.Pending;
    }

    public void MarkAsDelivered()
    {
        Status = OrderStatus.Delivered;
    }

    public void Reject()
    {
        foreach (var detail in OrderDetails)
        {
            detail.Variant.RestoreStock(detail.Quantity);
        }

        Status = OrderStatus.Rejected;
    }
}
