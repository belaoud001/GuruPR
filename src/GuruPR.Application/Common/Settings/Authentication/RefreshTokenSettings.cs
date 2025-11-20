namespace GuruPR.Application.Common.Settings.Authentication;

public class RefreshTokenSettings : ISettings
{
    public static string SectionName => "RefreshToken";

    public required int ExpirationTimeInDays { get; set; }
}
