using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Interfaces.Markers;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class OwnedGenericRepository<T> : GenericRepository<T>, IOwnedGenericRepository<T> where T : class, IOwnedEntity
{
    public OwnedGenericRepository(GuruDbContext guruDbContext) : base(guruDbContext)
    {
    }

    public async Task<T?> GetByIdAndOwnerAsync(string id, string ownerId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id && entity.UserId == ownerId, cancellationToken);

        return entity;
    }
}
