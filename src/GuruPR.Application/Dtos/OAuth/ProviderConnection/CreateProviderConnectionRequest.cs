namespace GuruPR.Application.Dtos.OAuth.ProviderConnection;

public class CreateProviderConnectionRequest
{
    public required string AccessToken { get; set; }

    public required string RefreshToken { get; set; }

    public required List<string> Scopes { get; set; }

    public required DateTime AccessExpiresAt { get; set; }

    public required DateTime CreatedAt { get; set; }
}
