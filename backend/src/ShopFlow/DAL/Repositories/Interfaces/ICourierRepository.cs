using ShopFlow.Models;

namespace ShopFlow.DAL.Repositories.Interfaces;

public interface ICourierRepository : IRepository<Courier>
{
    Task<bool> HasOrdersAsync(int courierId, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
