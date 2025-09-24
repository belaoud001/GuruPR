using Microsoft.EntityFrameworkCore;

using GuruPR.Application.Interfaces.Persistence;

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

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id) ?? throw new KeyNotFoundException();
    }

    public async Task<T> AddAsync(T entity)
    {
        var result = await _dbSet.AddAsync(entity);
        
        return result.Entity;
    }

    public void Update(T entity)
    {
        throw new NotImplementedException();
    }
    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
