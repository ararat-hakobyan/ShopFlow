using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ShopFlowDbContext _context;

    public UnitOfWork(ShopFlowDbContext context)
    {
        _context = context;

        Users = new UserRepository(context);
        Categories = new CategoryRepository(context);
        Products = new ProductRepository(context);
        Couriers = new CourierRepository(context);
        Orders = new OrderRepository(context);
        Baskets = new BasketRepository(context);
    }

    public IUserRepository Users { get; }

    public ICategoryRepository Categories { get; }

    public IProductRepository Products { get; }

    public ICourierRepository Couriers { get; }

    public IOrderRepository Orders { get; }

    public IBasketRepository Baskets { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
