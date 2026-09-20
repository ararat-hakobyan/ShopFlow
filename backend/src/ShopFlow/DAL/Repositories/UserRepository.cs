using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ShopFlowDbContext context) : base(context)
    {
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim();

        return Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == normalized, cancellationToken);
    }

    public Task<bool> ExistsAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim();
        var normalizedEmail = email.Trim();

        return Entities.AnyAsync(
            user => user.Username == normalizedUsername || user.Email == normalizedEmail,
            cancellationToken);
    }
}
