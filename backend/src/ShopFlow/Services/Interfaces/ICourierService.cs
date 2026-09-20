using ShopFlow.Common;
using ShopFlow.DTOModels;

namespace ShopFlow.Services.Interfaces;

public interface ICourierService
{
    Task<CourierConsoleDto> GetConsoleAsync(int courierId, CancellationToken cancellationToken = default);

    Task<Result> AcceptOrderAsync(int orderId, int courierId, CancellationToken cancellationToken = default);

    Task<Result> CompleteOrderAsync(int orderId, int courierId, CancellationToken cancellationToken = default);
}
