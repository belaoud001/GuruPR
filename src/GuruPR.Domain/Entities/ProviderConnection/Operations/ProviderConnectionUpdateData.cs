namespace GuruPR.Domain.Entities.ProviderConnection.Operations;

public class ProviderConnectionUpdateData
{
    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required List<string> Scopes { get; init; }
}
