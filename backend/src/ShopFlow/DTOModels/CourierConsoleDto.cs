namespace ShopFlow.DTOModels;

public sealed class CourierConsoleDto
{
    public List<OrderDto> AvailableOrders { get; set; } = new();

    public List<OrderDto> ActiveOrders { get; set; } = new();

    public List<OrderDto> DeliveredOrders { get; set; } = new();
}
