using GuruPR.Application.Settings;

namespace GuruPR.Application.Configuration.Security;

public class TokenEncryptionSettings : ISettings
{
    public static string SectionName => "TokenEncryption";

    public required string Key { get; set; }
}
