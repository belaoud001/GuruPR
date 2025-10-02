namespace GuruPR.Application.Configuration.Security;

public class TokenEncryption
{
    public const string SectionName = "TokenEncryption";

    public required string Key { get; set; }
}
