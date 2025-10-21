using GuruPR.Application.Settings.Email;
using GuruPR.Application.Settings.Extensions;
using GuruPR.Application.Settings.FrontEnd;
using GuruPR.Application.Settings.Security;
using GuruPR.Infrastructure.Configuration;
using GuruPR.Persistence.Configuration;

namespace GuruPR.Configuration;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(
            options => options.AddPolicy(
                "CorsPolicy",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader())
        );
    }

    public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettings<JwtSettings>(configuration);
        services.AddSettings<EmailValidationSettings>(configuration);
        services.AddSettings<GmailingAppSettings>(configuration);
        services.AddSettings<FrontEndSettings>(configuration);
    }

    public static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging();
    }

    public static void ConfigureSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
    }

    public static void ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
    }

    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)  
    {
        services.AddInfrastructureServices(configuration);
    }

    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistenceServices(configuration);
    }
}
