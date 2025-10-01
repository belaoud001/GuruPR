using GuruPR.Domain.Entities.Enums;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Dtos.OAuth.Provider;

namespace GuruPR.Application.Interfaces.Application;

public interface IProviderService
{
    Task<Provider> CreateProviderAsync(CreateProviderRequest createProviderRequest);

    Task<IEnumerable<Provider>> GetAllProvidersAsync();

    Task<Provider> GetProviderByIdAsync(string providerId);

    Task<Provider> GetProviderByNameAsync(string providerName);

    Task<Provider> GetProviderByTypeAsync(OAuthProviderType OAuthProviderType);

    Task<Provider> UpdateProviderAsync(string providerId, UpdateProviderRequest updateProviderRequest);

    Task<bool> DeleteProviderAsync(string providerId);
}
