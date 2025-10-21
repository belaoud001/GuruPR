namespace GuruPR.Application.Settings.Email;

public class GmailingAppSettings : ISettings
{
    public static string SectionName => "GmailingApp";

    public required string Host { get; set; }

    public int Port { get; set; } = 587;

    public required string Username { get; set; }

    public required string AppPassword { get; set; }
}
