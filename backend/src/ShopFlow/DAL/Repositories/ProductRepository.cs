using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;
using ShopFlow.Enums;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public sealed class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ShopFlowDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> ListWithVariantsAsync(
        string? search = null,
        int? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities
            .Include(product => product.Category)
            .Include(product => product.ProductVariants)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product => product.ProductName.Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryID == categoryId.Value);
        }

        return await query
            .OrderBy(product => product.ProductName)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetWithVariantsAsync(int productId, CancellationToken cancellationToken = default)
        => Entities
            .Include(product => product.ProductVariants)
            .FirstOrDefaultAsync(product => product.ProductID == productId, cancellationToken);

    public Task<ProductVariant?> GetVariantAsync(int variantId, CancellationToken cancellationToken = default)
        => Context.ProductVariants
            .Include(variant => variant.Product)
            .FirstOrDefaultAsync(variant => variant.VariantID == variantId, cancellationToken);

    public async Task<IReadOnlyList<ProductVariant>> ListVariantsAsync(
        int productId,
        CancellationToken cancellationToken = default)
        => await Context.ProductVariants
            .AsNoTracking()
            .Where(variant => variant.ProductID == productId)
            .OrderBy(variant => variant.Color)
            .ThenBy(variant => variant.Size)
            .ToListAsync(cancellationToken);

    public async Task AddVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default)
        => await Context.ProductVariants.AddAsync(variant, cancellationToken);

    public async Task<bool> HasActiveReferencesAsync(
        IEnumerable<int> variantIds,
        CancellationToken cancellationToken = default)
    {
        var ids = variantIds.Distinct().ToList();

        if (ids.Count == 0)
        {
            return false;
        }

        var isInSomeBasket = await Context.BasketItems
            .AnyAsync(item => ids.Contains(item.VariantID), cancellationToken);

        if (isInSomeBasket)
        {
            return true;
        }

        return await Context.OrderDetails
            .AnyAsync(
                detail => ids.Contains(detail.VariantID)
                          && detail.Order.Status != OrderStatus.Delivered
                          && detail.Order.Status != OrderStatus.Rejected,
                cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Entities.CountAsync(cancellationToken);
}
