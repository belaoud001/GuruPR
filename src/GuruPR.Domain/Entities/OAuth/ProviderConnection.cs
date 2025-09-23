namespace GuruPR.Domain.Entities.OAuth;

/// <summary>
/// Represents a connection to an OAuth provider, including tokens, scopes, and expiration details.
/// </summary>
public class ProviderConnection
{
    /// <summary>
    /// Unique identifier for the provider connection.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifier of the OAuth provider.
    /// </summary>
    public int ProviderId { get; set; }

    /// <summary>
    /// Access token used for authentication.
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh token used to renew the access token.
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// List of scopes granted for this connection.
    /// </summary>
    public required List<string> Scopes { get; set; }

    /// <summary>
    /// Expiration date and time of the access token.
    /// </summary>
    public required DateTime AccessExpiresAt { get; set; }

    /// <summary>
    /// Creation date and time of the provider connection.
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    public bool IsTokenExpired() => DateTime.UtcNow >= AccessExpiresAt;

    public bool HasScope(string scope) => Scopes.Contains(scope);

    public bool ShouldRefreshToken() => IsTokenExpired() && !string.IsNullOrEmpty(RefreshToken);
}
