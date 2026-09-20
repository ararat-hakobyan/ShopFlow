namespace ShopFlow.DAL.Repositories.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    ICategoryRepository Categories { get; }

    IProductRepository Products { get; }

    ICourierRepository Couriers { get; }

    IOrderRepository Orders { get; }

    IBasketRepository Baskets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
