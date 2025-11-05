namespace GuruPR.Domain.Entities.OAuth;

/// <summary>
/// Represents a connection to an OAuth provider, including tokens, scopes, and expiration details.
/// </summary>
public class ProviderConnection
{
    /// <summary>
    /// Unique identifier of the provider connection.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Client ID used for OAuth authentication.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Client secret used for OAuth authentication.
    /// </summary>
    public required string ClientSecret { get; set; }


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
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public bool IsTokenExpired(int bufferSeconds) => DateTime.UtcNow >= AccessExpiresAt.AddSeconds(-bufferSeconds);

    public bool HasScope(string scope) => Scopes.Any(selectedScope => string.Equals(selectedScope, scope, StringComparison.OrdinalIgnoreCase));

    public bool ShouldRefreshToken(int bufferSeconds = 60) => IsTokenExpired(bufferSeconds) && !string.IsNullOrEmpty(RefreshToken);
}
