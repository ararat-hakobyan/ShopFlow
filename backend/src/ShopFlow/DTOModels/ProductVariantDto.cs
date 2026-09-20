namespace ShopFlow.DTOModels;

public sealed class ProductVariantDto
{
    public int VariantId { get; set; }

    public int ProductId { get; set; }

    public string Color { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string Label => $"{Color} / {Size}";

    public bool IsInStock => StockQuantity > 0;
}
