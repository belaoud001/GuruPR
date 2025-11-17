using GuruPR.Domain.Entities.Provider.Enums;

namespace GuruPR.Domain.Entities.Provider.Operations;

public class ProviderUpdateData
{
    public string? DisplayName { get; init; }

    public OAuthProviderType? ProviderType { get; init; }

    public string? AuthorizationUrl { get; init; }

    public string? TokenUrl { get; init; }
}
