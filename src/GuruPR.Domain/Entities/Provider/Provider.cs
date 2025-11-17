using GuruPR.Domain.Entities.Provider.Enums;
using GuruPR.Domain.Entities.Provider.Operations;

namespace GuruPR.Domain.Entities.Provider;

/// <summary>
/// Represents an OAuth provider with its configuration details.
/// </summary>
public class Provider
{
    /// <summary>
    /// Unique identifier (GUID) of the provider.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Display name of the provider.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Prefixed provider type (e.g., Google, Discord).
    /// </summary>
    public required OAuthProviderType ProviderType { get; set; }

    /// <summary>
    /// URL for the authorization endpoint.
    /// </summary>
    public required string AuthorizationUrl { get; set; }

    /// <summary>
    /// URL for the token endpoint.
    /// </summary>
    public required string TokenUrl { get; set; }

    /// <summary>
    /// Default scopes used for OAuth authentication.
    /// </summary>
    public required List<string> DefaultScopes { get; set; }

    /// <summary>
    /// Date and time when the provider was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validates the provider configuration.
    /// </summary>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(DisplayName)) return false;
        if (!Enum.IsDefined(ProviderType)) return false;
        if (!Uri.IsWellFormedUriString(AuthorizationUrl, UriKind.Absolute)) return false;
        if (!Uri.IsWellFormedUriString(TokenUrl, UriKind.Absolute)) return false;
        if (DefaultScopes == null || DefaultScopes.Count == 0) return false;

        return true;
    }

    public void Update(ProviderUpdateData providerUpdateData)
    {
        DisplayName = providerUpdateData.DisplayName!;
        ProviderType = providerUpdateData.ProviderType ?? OAuthProviderType.Other;
        AuthorizationUrl = providerUpdateData.AuthorizationUrl!;
        TokenUrl = providerUpdateData.TokenUrl!;
    }
}
