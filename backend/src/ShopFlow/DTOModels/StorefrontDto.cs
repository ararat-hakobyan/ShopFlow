namespace ShopFlow.DTOModels;

public sealed class StorefrontDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime MemberSince { get; set; }

    public List<CategoryDto> Categories { get; set; } = new();

    public List<ProductDto> Products { get; set; } = new();

    public BasketDto Basket { get; set; } = new();

    public List<OrderDto> Orders { get; set; } = new();

    public int? SelectedCategoryId { get; set; }

    public int? SelectedProductId { get; set; }

    public List<ProductVariantDto> SelectedProductVariants { get; set; } = new();
}
