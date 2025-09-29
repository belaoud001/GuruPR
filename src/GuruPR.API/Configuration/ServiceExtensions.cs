using GuruPR.Persistence.Configuration;
using GuruPR.Infrastructure.Configuration;
using GuruPR.Application.Configuration.Extensions;

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

    public static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging();
    }

    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication().AddJwtBearer();
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder();
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
