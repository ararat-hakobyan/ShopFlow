using ShopFlow.Models;
using ShopFlow.Enums;
using ShopFlow.Common;

namespace ShopFlow.DAL.Repositories.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<PagedResult<Order>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<Order>> ListByUserAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> ListUnassignedAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> ListByCourierAsync(int courierId, OrderStatus status, CancellationToken cancellationToken = default);

    Task<PagedResult<Order>> ListDeliveredByCourierAsync(int courierId, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<Order?> GetWithLinesAsync(int orderId, CancellationToken cancellationToken = default);

    Task<Order?> GetForUpdateWithLinesAsync(int orderId, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<int> CountByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
}
