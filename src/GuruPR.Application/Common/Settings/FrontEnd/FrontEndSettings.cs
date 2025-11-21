namespace GuruPR.Application.Common.Settings.FrontEnd;

public class FrontEndSettings : ISettings
{
    public static string SectionName => "FrontEnd";

    public required string BaseUrl { get; init; }

    public string? EmailConfirmationPath { get; init; } = "/confirm-email";

    public string? EmailConfirmationFailedPath { get; init; } = "/confirm-email-failed";
}
