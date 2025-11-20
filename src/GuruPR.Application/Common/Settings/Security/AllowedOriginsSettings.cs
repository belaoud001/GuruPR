namespace GuruPR.Application.Common.Settings.Security;

public class AllowedOriginsSettings : ISettings
{
    public static string SectionName => "AllowedOrigins";

    public required IReadOnlyList<string> Origins { get; init; }
}
