namespace ShopFlow.DTOModels;

public sealed class AddVariantRequest
{
    public int ProductId { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }
}
