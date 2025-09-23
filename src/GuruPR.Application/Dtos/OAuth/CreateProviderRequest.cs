namespace GuruPR.Application.Dtos.OAuth;

public class CreateProviderRequest
{
    public required string Name { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string AuthorizationUrl { get; set; }

    public required string TokenUrl { get; set; }

    public required List<string> DefaultScopes { get; set; }
}
