using Microsoft.EntityFrameworkCore;
using ShopFlow.DAL.Repositories.Interfaces;

namespace ShopFlow.DAL.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected Repository(ShopFlowDbContext context)
    {
        Context = context;
    }

    protected ShopFlowDbContext Context { get; }

    protected DbSet<TEntity> Entities => Context.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await Entities.FindAsync(new object[] { id }, cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
        => await Entities.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await Entities.AddAsync(entity, cancellationToken);

    public virtual void Remove(TEntity entity) => Entities.Remove(entity);
}
