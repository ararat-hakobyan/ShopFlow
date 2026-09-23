using ShopFlow.Common;
using ShopFlow.DTOModels;

namespace ShopFlow.Services.Interfaces;

public interface IShopService
{
    Task<Result<StorefrontDto>> GetStorefrontAsync(
        int userId,
        int? categoryId = null,
        int? productId = null,
        CancellationToken cancellationToken = default);

    Task<Result> AddToCartAsync(int userId, AddToCartRequest request, CancellationToken cancellationToken = default);

    Task<Result> RemoveFromCartAsync(int userId, RemoveFromCartRequest request, CancellationToken cancellationToken = default);

    Task<Result<int>> PlaceOrderAsync(int userId, CancellationToken cancellationToken = default);

    Task<Result<OrderDto>> GetOrderForUserAsync(int userId, int orderId, CancellationToken cancellationToken = default);

    Task<Result> CancelOrderAsync(int userId, int orderId, CancellationToken cancellationToken = default);
}
