using GuruPR.Domain.Entities.ProviderConnection;

namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IProviderConnectionRepository : IGenericRepository<ProviderConnection>
{
    Task DeleteProviderConnectionsByProviderIdAsync(string providerId, CancellationToken cancellationToken = default);
}
