using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Domain.Entities.ProviderConnection;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class ProviderConnectionRepository : GenericRepository<ProviderConnection>, IProviderConnectionRepository
{
    public ProviderConnectionRepository(GuruDbContext guruDBContext) : base(guruDBContext)
    {
    }

    public async Task DeleteProviderConnectionsByProviderIdAsync(string providerId, CancellationToken cancellationToken = default)
    {
        await _dbSet.Where(providerConnection => providerConnection.ProviderId == providerId)
                    .ExecuteDeleteAsync(cancellationToken);
    }
}
