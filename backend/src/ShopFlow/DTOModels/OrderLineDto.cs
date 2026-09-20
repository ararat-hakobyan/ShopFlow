namespace ShopFlow.DTOModels;

public sealed class OrderLineDto
{
    public int VariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
