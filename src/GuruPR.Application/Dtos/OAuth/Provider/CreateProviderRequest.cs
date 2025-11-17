using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Domain.Entities.Provider.Enums;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class CreateProviderRequest
{
    public required string DisplayName { get; init; }

    public OAuthProviderType ProviderType { get; init; }

    public required string AuthorizationUrl { get; init; }

    public required string TokenUrl { get; init; }

    public List<string>? DefaultScopes { get; init; } = [];

    public List<CreateProviderConnectionRequest>? ProviderConnections { get; init; } = [];
}
