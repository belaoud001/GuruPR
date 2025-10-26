namespace GuruPR.Application.Dtos.OAuth.ProviderConnection;

public class ProviderConnectionDto
{
    /// <summary>
    /// Unique identifier (GUID) of the provider connection.
    /// </summary>
    public string Id { get; init; } = null!;

    /// <summary>
    /// Access token used for authentication.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Client ID used for OAuth authentication.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// Refresh token used to renew the access token.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// List of scopes granted for this connection.
    /// </summary>
    public required List<string> Scopes { get; init; }

    /// <summary>
    /// Expiration date and time of the access token.
    /// </summary>
    public required DateTime AccessExpiresAt { get; init; }

    /// <summary>
    /// Creation date and time of the provider connection.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
}
