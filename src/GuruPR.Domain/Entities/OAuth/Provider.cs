namespace GuruPR.Domain.Entities.OAuth;

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
    /// Name of the provider.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Client ID used for OAuth authentication.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Client secret used for OAuth authentication.
    /// </summary>
    public required string ClientSecret { get; set; }

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
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// List of connections associated with this provider.
    /// </summary>
    public List<ProviderConnection> ProviderConnections { get; set; } = [];

    /// <summary>
    /// Validates the provider configuration.
    /// </summary>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Name)) return false;
        if (string.IsNullOrWhiteSpace(ClientId)) return false;
        if (string.IsNullOrWhiteSpace(ClientSecret)) return false;
        if (!Uri.IsWellFormedUriString(AuthorizationUrl, UriKind.Absolute)) return false;
        if (!Uri.IsWellFormedUriString(TokenUrl, UriKind.Absolute)) return false;
        if (DefaultScopes == null || DefaultScopes.Count == 0) return false;

        return true;
    }

    public void AddProviderConnection(ProviderConnection providerConnection)
    {
        if (providerConnection == null)
        {
            throw new ArgumentNullException(nameof(providerConnection));
        }

        ProviderConnections.Add(providerConnection);
    }

    public bool RemoveProviderConnection(string providerConnectionId)
    {
        if (string.IsNullOrWhiteSpace(providerConnectionId))
        {
            throw new ArgumentException("Provider connection ID cannot be null or empty.", nameof(providerConnectionId));
        }

        var providerConnection = ProviderConnections.FirstOrDefault(providerConnection => providerConnection.Id == providerConnectionId);
        if (providerConnection != null)
        {
            return ProviderConnections.Remove(providerConnection);
        }

        return false;
    }
}
