using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ShopFlowDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default)
        => await Entities
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim();
        return Entities.AnyAsync(category => category.Name == normalized, cancellationToken);
    }

    public Task<bool> ExistsAsync(int categoryId, CancellationToken cancellationToken = default)
        => Entities.AnyAsync(category => category.CategoryID == categoryId, cancellationToken);
}
