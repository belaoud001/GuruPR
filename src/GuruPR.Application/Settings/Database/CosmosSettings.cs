namespace GuruPR.Application.Settings.Database;

public class CosmosSettings
{
    public const string SectionName = "Cosmos";
    public required string AccountEndpoint { get; init; }

    public required string AccountKey { get; init; }

    public required string DatabaseName { get; init; }
}
