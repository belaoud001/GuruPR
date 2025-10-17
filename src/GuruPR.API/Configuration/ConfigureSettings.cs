using GuruPR.Application.Settings;
using GuruPR.Application.Settings.Security;

namespace GuruPR.Configuration;

public static class ConfigureSettings
{
    public static void AddSettings<T>(this IServiceCollection services, IConfiguration configuration) where T : class, ISettings
    {
        services.Configure<T>(configuration.GetSection(T.SectionName));
    }
}
