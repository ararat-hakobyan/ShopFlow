using ShopFlow.Enums;

namespace ShopFlow.DTOModels;

public sealed class UpdateOrderStatusRequest
{
    public int OrderId { get; set; }

    public OrderStatus Status { get; set; }

    public int? CourierId { get; set; }
}
