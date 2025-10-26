namespace GuruPR.Application.Settings.Database;

public class PostgresSettings
{
    public const string SectionName = "PostgreSQL";

    public string ConnectionString { get; init; } = null!;
}
