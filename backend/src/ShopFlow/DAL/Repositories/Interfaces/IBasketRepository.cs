using ShopFlow.Models;

namespace ShopFlow.DAL.Repositories.Interfaces;

public interface IBasketRepository : IRepository<Basket>
{
    Task<Basket?> GetWithItemsAsync(int userId, CancellationToken cancellationToken = default);

    Task<Basket> GetOrCreateAsync(int userId, CancellationToken cancellationToken = default);

    void RemoveItem(BasketItem item);

    void RemoveItems(IEnumerable<BasketItem> items);

    Task RemoveItemsWithVariantsAsync(IEnumerable<int> variantIds, CancellationToken cancellationToken = default);
}
