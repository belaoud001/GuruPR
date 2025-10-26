using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Domain.Entities.Enums;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class ProviderDto
{
    public string Id { get; init; } = null!;

    public string DisplayName { get; init; } = null!;

    public OAuthProviderType ProviderType { get; init; }

    public string AuthorizationUrl { get; init; } = null!;

    public string TokenUrl { get; init; } = null!;

    public IReadOnlyList<string> DefaultScopes { get; init; } = [];

    public DateTime CreatedAt { get; init; }

    public List<ProviderConnectionDto> ProviderConnections { get; init; } = [];
}
