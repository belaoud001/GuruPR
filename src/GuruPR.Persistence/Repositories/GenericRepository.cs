using System.Linq.Expressions;

using GuruPR.Application.Common.Interfaces.Persistence;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public IQueryable<T> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
                          .AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken); ;
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.AddAsync(entity, cancellationToken);

        return result.Entity;
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
