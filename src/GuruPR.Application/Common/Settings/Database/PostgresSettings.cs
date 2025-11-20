namespace GuruPR.Application.Common.Settings.Database;

public class PostgresSettings
{
    public const string SectionName = "PostgreSQL";

    public string ConnectionString { get; init; } = null!;
}
