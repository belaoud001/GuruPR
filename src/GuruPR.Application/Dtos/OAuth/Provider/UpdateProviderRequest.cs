using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class UpdateProviderRequest
{
    public required string Name { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string AuthorizationUrl { get; set; }

    public required string TokenUrl { get; set; }

    public required List<string> DefaultScopes { get; set; }
}
