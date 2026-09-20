namespace ShopFlow.DTOModels;

public sealed class BasketDto
{
    public List<BasketItemDto> Items { get; set; } = new();

    public decimal Total => Items.Sum(item => item.LineTotal);

    public int ItemCount => Items.Count;

    public bool IsEmpty => Items.Count == 0;
}
