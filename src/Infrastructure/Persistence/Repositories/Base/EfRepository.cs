

namespace Infrastructure.Persistence.Repositories.Base;

public class EfRepository<T, TId>(AppDbContext dbContext) : IRepository<T, TId>
    where T : BaseEntity<TId>
{
    protected AppDbContext DbContext => dbContext;

    public async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().Where(predicate).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await dbContext.Set<T>().CountAsync(cancellationToken)
            : await dbContext.Set<T>().CountAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
    }

    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        dbContext.Set<T>().UpdateRange(entities);
    }

    public void Remove(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        dbContext.Set<T>().RemoveRange(entities);
    }

    public IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null, bool asNoTracking = true)
    {
        var query = dbContext.Set<T>().AsQueryable();

        if (predicate is not null)
            query = query.Where(predicate);

        return asNoTracking ? query.AsNoTracking() : query;
    }
}