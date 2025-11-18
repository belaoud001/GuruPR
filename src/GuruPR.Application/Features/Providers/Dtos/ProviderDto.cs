using GuruPR.Domain.Entities.Provider.Enums;

namespace GuruPR.Application.Features.Providers.Dtos;

public class ProviderDto
{
    public string Id { get; init; } = null!;

    public string DisplayName { get; init; } = null!;

    public OAuthProviderType ProviderType { get; init; }

    public string AuthorizationUrl { get; init; } = null!;

    public string TokenUrl { get; init; } = null!;

    public IReadOnlyList<string> DefaultScopes { get; init; } = [];

    public DateTime CreatedAt { get; init; }
}
