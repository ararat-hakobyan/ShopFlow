namespace ShopFlow.DTOModels;

public sealed class UpdateVariantPriceRequest
{
    public int VariantId { get; set; }

    public decimal NewPrice { get; set; }
}
