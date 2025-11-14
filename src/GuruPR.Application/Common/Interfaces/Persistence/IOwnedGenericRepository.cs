using GuruPR.Domain.Interfaces.Markers;

namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IOwnedGenericRepository<T> : IGenericRepository<T> where T : class, IOwnedEntity
{
    Task<T?> GetByIdAndOwnerAsync(string id, string ownerId, CancellationToken cancellationToken = default);
}
