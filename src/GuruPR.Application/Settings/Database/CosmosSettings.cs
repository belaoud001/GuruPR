namespace GuruPR.Application.Settings.Database;

public class CosmosSettings
{
    public const string SectionName = "Cosmos";
    public required string AccountEndpoint { get; set; }

    public required string AccountKey { get; set; }

    public required string DatabaseName { get; set; }
}
