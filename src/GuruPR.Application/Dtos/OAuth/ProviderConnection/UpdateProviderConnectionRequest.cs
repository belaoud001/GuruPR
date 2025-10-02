namespace GuruPR.Application.Dtos.OAuth.ProviderConnection;

public class UpdateProviderConnectionRequest
{

    /// <summary>
    /// Client ID used for OAuth authentication.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Client secret used for OAuth authentication.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Access token used for authentication.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Refresh token used to renew the access token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// List of scopes granted for this connection.
    /// </summary>
    public List<string>? Scopes { get; set; }

    /// <summary>
    /// Expiration date and time of the access token.
    /// </summary>
    public DateTime? AccessExpiresAt { get; set; }

    /// <summary>
    /// Creation date and time of the provider connection.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}
