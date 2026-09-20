using ShopFlow.Models;

namespace ShopFlow.DAL.Repositories.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> ListWithVariantsAsync(
        string? search = null,
        int? categoryId = null,
        CancellationToken cancellationToken = default);

    Task<Product?> GetWithVariantsAsync(int productId, CancellationToken cancellationToken = default);

    Task<ProductVariant?> GetVariantAsync(int variantId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductVariant>> ListVariantsAsync(int productId, CancellationToken cancellationToken = default);

    Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default);

    Task<bool> HasActiveReferencesAsync(IEnumerable<int> variantIds, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
