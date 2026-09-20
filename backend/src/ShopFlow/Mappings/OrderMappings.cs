using ShopFlow.DTOModels;
using ShopFlow.Models;

namespace ShopFlow.Mappings;

public static class OrderMappings
{
    public static OrderLineDto ToDto(this OrderDetail detail)
        => new()
        {
            VariantId = detail.VariantID,
            ProductName = detail.Variant?.Product?.ProductName ?? "Unavailable product",
            Color = detail.Variant?.Color ?? "-",
            Size = detail.Variant?.Size ?? "-",
            Quantity = detail.Quantity,
            UnitPrice = detail.PriceAtPurchase
        };

    public static OrderDto ToDto(this Order order)
        => new()
        {
            OrderId = order.OrderID,
            UserId = order.UserID,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CourierId = order.CourierID,
            CourierName = order.Courier?.FullName,
            CourierPhone = order.Courier?.Phone,
            CourierCoordinates = order.CourierCoordinates,
            EstimatedDeliveryTime = order.EstimatedDeliveryTime,
            CreatedAt = order.CreatedAt,
            Lines = order.OrderDetails.Select(ToDto).ToList()
        };

    public static List<OrderDto> ToDtoList(this IEnumerable<Order> orders)
        => orders.Select(ToDto).ToList();
}
