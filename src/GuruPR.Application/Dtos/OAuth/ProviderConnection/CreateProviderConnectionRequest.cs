namespace GuruPR.Application.Dtos.OAuth.ProviderConnection;

public class CreateProviderConnectionRequest
{
    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required List<string> Scopes { get; init; }

    public required DateTime AccessExpiresAt { get; init; }

    public required DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
