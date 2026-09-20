namespace ShopFlow.DTOModels;

public sealed class AddProductRequest
{
    public string ProductName { get; set; } = string.Empty;

    public int CategoryId { get; set; }
}
