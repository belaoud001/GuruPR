using GuruPR.Application.Common.Settings.Authentication.Providers;

namespace GuruPR.Application.Common.Settings.Authentication;

public class ExternalAuthentication
{
    public static string SectionName => "ExternalAuthentication";

    public required GoogleProvider Google { get; init; }
}
