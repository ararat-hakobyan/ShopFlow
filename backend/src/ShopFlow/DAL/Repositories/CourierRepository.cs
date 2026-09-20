using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public sealed class CourierRepository : Repository<Courier>, ICourierRepository
{
    public CourierRepository(ShopFlowDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<Courier>> ListAsync(CancellationToken cancellationToken = default)
        => await Entities
            .AsNoTracking()
            .OrderBy(courier => courier.FirstName)
            .ThenBy(courier => courier.LastName)
            .ToListAsync(cancellationToken);

    public Task<bool> HasOrdersAsync(int courierId, CancellationToken cancellationToken = default)
        => Context.Orders.AnyAsync(order => order.CourierID == courierId, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Entities.CountAsync(cancellationToken);
}
