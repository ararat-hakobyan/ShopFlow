using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;
using ShopFlow.Enums;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ShopFlowDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Order>> SearchAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = WithLines().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = int.TryParse(term, out var orderId)
                ? query.Where(order => order.OrderID == orderId)
                : query.Where(order =>
                    order.Courier != null &&
                    (order.Courier.FirstName.Contains(term) || order.Courier.LastName.Contains(term)));
        }

        return await query
            .OrderByDescending(order => order.OrderID)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> ListByUserAsync(
        int userId,
        CancellationToken cancellationToken = default)
        => await WithLines()
            .Where(order => order.UserID == userId)
            .OrderByDescending(order => order.OrderID)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Order>> ListUnassignedAsync(CancellationToken cancellationToken = default)
        => await WithLines()
            .Where(order => order.CourierID == null && order.Status == OrderStatus.Pending)
            .OrderByDescending(order => order.OrderID)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Order>> ListByCourierAsync(
        int courierId,
        OrderStatus status,
        int? maxResults = null,
        CancellationToken cancellationToken = default)
    {
        var query = WithLines()
            .Where(order => order.CourierID == courierId && order.Status == status)
            .OrderByDescending(order => order.OrderID)
            .AsQueryable();

        if (maxResults.HasValue)
        {
            query = query.Take(maxResults.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public Task<Order?> GetWithLinesAsync(int orderId, CancellationToken cancellationToken = default)
        => WithLines().FirstOrDefaultAsync(order => order.OrderID == orderId, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => Entities.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
        => Entities.CountAsync(order => order.Status == status, cancellationToken);

    private IQueryable<Order> WithLines()
        => Entities
            .IgnoreQueryFilters()
            .Include(order => order.Courier)
            .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Variant)
                    .ThenInclude(variant => variant.Product)
            .AsNoTracking();
}
