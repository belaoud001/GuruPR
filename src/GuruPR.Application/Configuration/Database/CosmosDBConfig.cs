namespace GuruPR.Application.Configuration.Database;

public class CosmosDBConfig
{
    public required string AccountEndpoint { get; set; }

    public required string AccountKey { get; set; }

    public required string DatabaseName { get; set; }
}
