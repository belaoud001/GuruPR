namespace GuruPR.Application.Settings.Security;

public class TokenEncryptionSettings : ISettings
{
    public static string SectionName => "TokenEncryption";

    public required string Key { get; set; }
}
