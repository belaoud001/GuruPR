namespace GuruPR.Application.Common.Settings.Security;

public class EmailValidationSettings : ISettings
{
    public static string SectionName => "EmailValidation";

    public required HashSet<string> AllowedDomains { get; set; } = new HashSet<string>();
}
