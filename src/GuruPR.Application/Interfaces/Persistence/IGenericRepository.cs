namespace GuruPR.Application.Interfaces.Persistence;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<T> AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
}
