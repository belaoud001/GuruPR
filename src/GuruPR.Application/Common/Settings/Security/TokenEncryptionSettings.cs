namespace GuruPR.Application.Common.Settings.Security;

public class TokenEncryptionSettings : ISettings
{
    public static string SectionName => "TokenEncryption";

    public required string Key { get; set; }
}
