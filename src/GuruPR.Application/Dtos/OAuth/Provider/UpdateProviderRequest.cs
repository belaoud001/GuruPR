using GuruPR.Domain.Entities.Provider.Enums;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class UpdateProviderRequest
{
    public string? DisplayName { get; init; }

    public OAuthProviderType? ProviderType { get; init; }

    public string? AuthorizationUrl { get; init; }

    public string? TokenUrl { get; init; }

    public List<string>? DefaultScopes { get; init; }
}
