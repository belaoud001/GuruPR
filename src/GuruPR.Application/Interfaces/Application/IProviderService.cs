using GuruPR.Application.Dtos.OAuth.Provider;
using GuruPR.Domain.Entities.Enums;
using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Interfaces.Application;

public interface IProviderService
{
    Task<Provider> CreateProviderAsync(CreateProviderRequest createProviderRequest);

    Task<List<Provider>> GetAllProvidersAsync();

    Task<Provider> GetProviderByIdAsync(string providerId);

    Task<Provider> GetProviderByNameAsync(string providerName);

    Task<Provider> GetProviderByTypeAsync(OAuthProviderType oAuthProviderType);

    Task<Provider> UpdateProviderAsync(string providerId, UpdateProviderRequest updateProviderRequest);

    Task<bool> DeleteProviderAsync(string providerId);
}
