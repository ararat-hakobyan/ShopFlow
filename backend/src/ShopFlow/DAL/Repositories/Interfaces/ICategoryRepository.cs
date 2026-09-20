using ShopFlow.Models;

namespace ShopFlow.DAL.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int categoryId, CancellationToken cancellationToken = default);
}
