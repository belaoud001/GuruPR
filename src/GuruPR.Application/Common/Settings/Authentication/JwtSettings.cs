namespace GuruPR.Application.Common.Settings.Authentication;

public class JwtSettings : ISettings
{
    public static string SectionName => "Jwt";

    public required string SecretKey { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required int ExpirationTimeInMinutes { get; set; }
}
