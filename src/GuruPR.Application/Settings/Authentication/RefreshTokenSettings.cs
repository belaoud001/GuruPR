namespace GuruPR.Application.Settings.Authentication;

public class RefreshTokenSettings : ISettings
{
    public static string SectionName => "RefreshToken";

    public required int ExpirationTimeInDays { get; set; }
}
