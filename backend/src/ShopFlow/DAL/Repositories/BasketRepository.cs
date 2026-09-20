using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public sealed class BasketRepository : Repository<Basket>, IBasketRepository
{
    public BasketRepository(ShopFlowDbContext context) : base(context)
    {
    }

    public Task<Basket?> GetWithItemsAsync(int userId, CancellationToken cancellationToken = default)
        => Entities
            .Include(basket => basket.BasketItems)
                .ThenInclude(item => item.Variant)
                    .ThenInclude(variant => variant.Product)
            .FirstOrDefaultAsync(basket => basket.UserID == userId, cancellationToken);

    public async Task<Basket> GetOrCreateAsync(int userId, CancellationToken cancellationToken = default)
    {
        var basket = await GetWithItemsAsync(userId, cancellationToken);

        if (basket is not null)
        {
            return basket;
        }

        basket = new Basket { UserID = userId };
        await Entities.AddAsync(basket, cancellationToken);

        return basket;
    }

    public void RemoveItem(BasketItem item) => Context.BasketItems.Remove(item);

    public void RemoveItems(IEnumerable<BasketItem> items) => Context.BasketItems.RemoveRange(items);
}
