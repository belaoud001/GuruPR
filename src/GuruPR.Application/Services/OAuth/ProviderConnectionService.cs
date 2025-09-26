using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Application.Interfaces.Application;

namespace GuruPR.Application.Services.OAuth;

public class ProviderConnectionService : IProviderConnectionService
{
    public async Task<ProviderConnectionDto> AddProviderConnectionToProviderAsync(string providerId, CreateProviderConnectionRequest createProviderConnectionRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteProviderConnectionAsync(string providerId, string providerConnectionId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProviderConnectionDto>> GetConnectionsByProviderAsync(string providerId)
    {
        throw new NotImplementedException();
    }
}
