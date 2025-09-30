using Microsoft.EntityFrameworkCore;

using GuruPR.Persistence.Contexts;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Interfaces.Persistence;

namespace GuruPR.Persistence.Repositories;

public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
{
    private readonly GuruDBContext _guruDBContext;
    private readonly DbSet<Provider> _dbSet;

    public ProviderRepository(GuruDBContext guruDBContext) : base(guruDBContext)
    {
        _guruDBContext = guruDBContext;
        _dbSet = _guruDBContext.Set<Provider>();
    }

    public async Task<Provider> GetProviderByNameAsync(string providerName)
    {
        return await _dbSet.FirstOrDefaultAsync(provider => provider.Name == providerName);
    }
}
