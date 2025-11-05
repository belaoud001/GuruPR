namespace GuruPR.Application.Dtos.OAuth.ProviderConnection;

public class UpdateProviderConnectionRequest
{
    /// <summary>
    /// Client ID used for OAuth authentication.
    /// </summary>
    public string? ClientId { get; init; }

    /// <summary>
    /// Client secret used for OAuth authentication.
    /// </summary>
    public string? ClientSecret { get; init; }

    /// <summary>
    /// Access token used for authentication.
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// Refresh token used to renew the access token.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// List of scopes granted for this connection.
    /// </summary>
    public List<string>? Scopes { get; init; }

    /// <summary>
    /// Expiration date and time of the access token.
    /// </summary>
    public DateTime? AccessExpiresAt { get; init; }
}
