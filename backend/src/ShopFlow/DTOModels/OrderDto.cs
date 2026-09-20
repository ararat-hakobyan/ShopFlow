using ShopFlow.Enums;

namespace ShopFlow.DTOModels;

public sealed class OrderDto
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public int? CourierId { get; set; }

    public string? CourierName { get; set; }

    public string? CourierPhone { get; set; }

    public string? CourierCoordinates { get; set; }

    public DateTime? EstimatedDeliveryTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderLineDto> Lines { get; set; } = new();

    public int ItemCount => Lines.Sum(line => line.Quantity);
}
