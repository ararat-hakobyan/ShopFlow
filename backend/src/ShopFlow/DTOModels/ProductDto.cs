namespace ShopFlow.DTOModels;

public sealed class ProductDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public List<ProductVariantDto> Variants { get; set; } = new();
}
