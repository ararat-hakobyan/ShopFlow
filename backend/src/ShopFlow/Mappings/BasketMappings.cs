using ShopFlow.DTOModels;
using ShopFlow.Models;

namespace ShopFlow.Mappings;

public static class BasketMappings
{
    public static BasketItemDto ToDto(this BasketItem item)
        => new()
        {
            VariantId = item.VariantID,
            ProductName = item.Variant?.Product?.ProductName ?? "Unavailable product",
            Color = item.Variant?.Color ?? "-",
            Size = item.Variant?.Size ?? "-",
            Quantity = item.Quantity,
            UnitPrice = item.Variant?.Price ?? 0m,
            StockQuantity = item.Variant?.StockQuantity ?? 0
        };

    public static BasketDto ToDto(this Basket? basket)
        => new()
        {
            Items = basket is null
                ? new List<BasketItemDto>()
                : basket.BasketItems.Select(ToDto).ToList()
        };
}
