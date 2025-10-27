using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Domain.Entities.Enums;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class CreateProviderRequest
{
    public required string DisplayName { get; init; }

    public OAuthProviderType ProviderType { get; init; }

    public required string AuthorizationUrl { get; init; }

    public required string TokenUrl { get; init; }

    public required List<string> DefaultScopes { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public List<CreateProviderConnectionRequest> ProviderConnections { get; init; } = [];
}
