using GuruPR.Domain.Entities.Provider;
using GuruPR.Domain.Entities.Provider.Enums;

namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IProviderRepository : IGenericRepository<Provider>
{
    // TODO: Think of eager and lazy loading here.

    Task<Provider?> GetProviderByNameAsync(string providerName);

    Task<Provider?> GetProviderByTypeAsync(OAuthProviderType oAuthProviderType);
}
