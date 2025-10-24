namespace GuruPR.Application.Settings.Security;

public class TokenHashingSettings : ISettings
{
    public static string SectionName => "TokenHashing";

    public required string Key { get; init; }
}
