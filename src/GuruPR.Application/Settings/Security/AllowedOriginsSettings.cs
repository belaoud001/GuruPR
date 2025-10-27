namespace GuruPR.Application.Settings.Security;

public class AllowedOriginsSettings : ISettings
{
    public static string SectionName => "AllowedOrigins";

    public required IReadOnlyList<string> Origins { get; init; }
}
