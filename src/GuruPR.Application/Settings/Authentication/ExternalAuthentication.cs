using GuruPR.Application.Settings.Authentication.Providers;

namespace GuruPR.Application.Settings.Authentication;

public class ExternalAuthentication
{
    public static string SectionName => "ExternalAuthentication";

    public required GoogleProvider Google { get; init; }
}
