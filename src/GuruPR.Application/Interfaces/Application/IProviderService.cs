using GuruPR.Application.Dtos.OAuth.Provider;

namespace GuruPR.Application.Interfaces.Application;

public interface IProviderService
{
    Task<ProviderDto> CreateProviderAsync(CreateProviderRequest createProviderRequest);

    Task<IEnumerable<ProviderDto>> GetAllProvidersAsync();

    Task<ProviderDto> GetProviderByIdAsync(string providerId);

    Task<ProviderDto> GetProviderByNameAsync(string providerName);

    Task<ProviderDto> UpdateProviderAsync(string providerId, UpdateProviderRequest updateProviderRequest);

    Task<bool> DeleteProviderAsync(string providerId);
}
