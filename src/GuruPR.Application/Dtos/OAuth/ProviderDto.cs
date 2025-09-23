namespace GuruPR.Application.Dtos.OAuth;

public class ProviderDto
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string ClientId { get; set; } = default!;

    public string AuthorizationUrl { get; set; } = default!;

    public string TokenUrl { get; set; } = default!;

    public List<string> DefaultScopes { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}
