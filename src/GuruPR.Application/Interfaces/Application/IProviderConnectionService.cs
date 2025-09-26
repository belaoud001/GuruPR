using GuruPR.Application.Dtos.OAuth.ProviderConnection;

namespace GuruPR.Application.Interfaces.Application;

public interface IProviderConnectionService
{
    Task<IEnumerable<ProviderConnectionDto>> GetConnectionsByProviderAsync(string providerId);

    Task<ProviderConnectionDto> AddProviderConnectionToProviderAsync(string providerId, CreateProviderConnectionRequest createProviderConnectionRequest);

    Task<bool> DeleteProviderConnectionAsync(string providerId, string providerConnectionId);
}
