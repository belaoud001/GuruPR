using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Domain.Entities.Enums;
using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Common.Interfaces.Application;

public interface IProviderConnectionService
{
    Task<List<ProviderConnection>> GetConnectionsByProviderIdAsync(string providerId);

    Task<ProviderConnection> GetProviderConnectionByIdAsync(string providerId, string providerConnectionId);

    Task<ProviderConnection> GetProviderConnectionByScopeAndProviderNameAsync(string providerName, string scope);

    Task<ProviderConnection> GetProviderConnectionByProviderTypeAndScopeAsync(OAuthProviderType OAuthProviderType, string scope);

    Task<ProviderConnection> AddProviderConnectionToProviderAsync(string providerId, CreateProviderConnectionRequest createProviderConnectionRequest);

    Task<ProviderConnection> UpdateProviderConnectionByProviderTypeAsync(OAuthProviderType OAuthProviderType, string providerConnectionId, UpdateProviderConnectionRequest updateProviderConnectionRequest);

    Task<bool> DeleteProviderConnectionAsync(string providerId, string providerConnectionId);
}
