using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Domain.Entities.Enums;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class CreateProviderRequest
{
    public required string DisplayName { get; set; }

    public OAuthProviderType ProviderType { get; set; }

    public required string AuthorizationUrl { get; set; }

    public required string TokenUrl { get; set; }

    public required List<string> DefaultScopes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CreateProviderConnectionRequest> ProviderConnections { get; set; } = [];
}
