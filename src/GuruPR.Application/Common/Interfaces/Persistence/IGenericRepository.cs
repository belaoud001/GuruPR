namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> AsQueryable();

    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);
}
