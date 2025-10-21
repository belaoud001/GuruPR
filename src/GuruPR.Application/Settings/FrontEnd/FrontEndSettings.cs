namespace GuruPR.Application.Settings.FrontEnd;

public class FrontEndSettings : ISettings
{
    public static string SectionName => "FrontEnd";

    public required string BaseUrl { get; set; }

    public string? EmailConfirmationPath => "/confirm-email";

    public string? EmailConfirmationFailedPath => "/confirm-email-failed";
}
