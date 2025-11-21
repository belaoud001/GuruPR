namespace GuruPR.Application.Common.Settings.Email;

public class GmailingAppSettings : ISettings
{
    public static string SectionName => "GmailingApp";

    public required string Host { get; init; }

    public int Port { get; init; } = 587;

    public required string Username { get; init; }

    public required string AppPassword { get; init; }
}
