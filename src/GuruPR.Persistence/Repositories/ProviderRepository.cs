using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Enums;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
{
    public ProviderRepository(GuruDbContext guruDBContext) : base(guruDBContext)
    {
    }

    public async Task<Provider?> GetProviderByNameAsync(string providerName)
    {
        var provider = await _dbSet.FirstOrDefaultAsync(provider => provider.DisplayName == providerName);

        return provider;
    }

    public async Task<Provider?> GetProviderByTypeAsync(OAuthProviderType OAuthProviderType)
    {
        var provider = await _dbSet.FirstOrDefaultAsync(provider => provider.ProviderType == OAuthProviderType);

        return provider;
    }
}
