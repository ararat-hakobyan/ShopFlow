namespace ShopFlow.DTOModels;

public sealed class AdminDashboardDto
{
    public int TotalOrders { get; set; }

    public int PendingOrders { get; set; }

    public int TotalProducts { get; set; }

    public int TotalCouriers { get; set; }

    public List<ProductDto> Products { get; set; } = new();

    public List<CategoryDto> Categories { get; set; } = new();

    public List<CourierDto> Couriers { get; set; } = new();

    public string? SearchQuery { get; set; }
}
