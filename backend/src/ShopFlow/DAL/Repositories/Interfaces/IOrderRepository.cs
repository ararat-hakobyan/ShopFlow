using ShopFlow.Models;
using ShopFlow.Enums;

namespace ShopFlow.DAL.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> SearchAsync(string? search, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> ListByUserAsync(int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> ListUnassignedAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> ListByCourierAsync(
        int courierId,
        OrderStatus status,
        int? maxResults = null,
        CancellationToken cancellationToken = default);

    Task<Order?> GetWithLinesAsync(int orderId, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<int> CountByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
}
