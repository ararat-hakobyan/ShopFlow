using ShopFlow.Models;

namespace ShopFlow.DAL.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken = default);
}
