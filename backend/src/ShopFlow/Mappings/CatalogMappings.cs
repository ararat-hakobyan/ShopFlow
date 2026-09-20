using ShopFlow.DTOModels;
using ShopFlow.Models;

namespace ShopFlow.Mappings;

public static class CatalogMappings
{
    public static CategoryDto ToDto(this Category category)
        => new()
        {
            CategoryId = category.CategoryID,
            Name = category.Name
        };

    public static ProductVariantDto ToDto(this ProductVariant variant)
        => new()
        {
            VariantId = variant.VariantID,
            ProductId = variant.ProductID,
            Color = variant.Color,
            Size = variant.Size,
            Price = variant.Price,
            StockQuantity = variant.StockQuantity
        };

    public static ProductDto ToDto(this Product product)
        => new()
        {
            ProductId = product.ProductID,
            ProductName = product.ProductName,
            CategoryId = product.CategoryID,
            CategoryName = product.Category?.Name ?? string.Empty,
            Variants = product.ProductVariants.Select(ToDto).ToList()
        };

    public static List<CategoryDto> ToDtoList(this IEnumerable<Category> categories)
        => categories.Select(ToDto).ToList();

    public static List<ProductDto> ToDtoList(this IEnumerable<Product> products)
        => products.Select(ToDto).ToList();

    public static List<ProductVariantDto> ToDtoList(this IEnumerable<ProductVariant> variants)
        => variants.Select(ToDto).ToList();
}
