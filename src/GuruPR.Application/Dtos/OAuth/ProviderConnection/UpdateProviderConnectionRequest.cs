namespace GuruPR.Application.Dtos.OAuth.ProviderConnection;

public class UpdateProviderConnectionRequest
{
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
}
