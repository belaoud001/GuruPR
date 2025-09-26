using GuruPR.Application.Dtos.OAuth.ProviderConnection;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class ProviderDto
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string ClientId { get; set; } = default!;

    public string AuthorizationUrl { get; set; } = default!;

    public string TokenUrl { get; set; } = default!;

    public List<string> DefaultScopes { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public List<ProviderConnectionDto> ProviderConnections { get; set; } = [];
}
