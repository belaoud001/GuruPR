using GuruPR.Application.Dtos.OAuth.ProviderConnection;

namespace GuruPR.Application.Interfaces.Application;

public interface IProviderConnectionService
{
    Task<IEnumerable<ProviderConnectionDto>> GetConnectionsByProviderIdAsync(string providerId);

    Task<ProviderConnectionDto> GetProviderConnectionByIdAsync(string providerId, string providerConnectionId);

    Task<ProviderConnectionDto> GetProviderConnectionByScopeAndProviderNameAsync(string providerName, string scope);

    Task<ProviderConnectionDto> AddProviderConnectionToProviderAsync(string providerId, CreateProviderConnectionRequest createProviderConnectionRequest);

    Task<bool> DeleteProviderConnectionAsync(string providerId, string providerConnectionId);
}
