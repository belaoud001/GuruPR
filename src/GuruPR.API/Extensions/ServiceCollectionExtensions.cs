using Asp.Versioning;

using GuruPR.Application.Extensions;
using GuruPR.Application.Settings;
using GuruPR.Application.Settings.Email;
using GuruPR.Application.Settings.FrontEnd;
using GuruPR.Application.Settings.Security;
using GuruPR.Infrastructure.Extensions;
using GuruPR.Persistence.Extensions;

namespace GuruPR.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAllApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureVersioning();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer(
                    (document, context, cancellationToken) =>
                    {
                        document.Info.Title = "GuruPR API";
                        document.Info.Version = "v1";
                        document.Info.Description = "GuruPR API Version 1.0";

                        Console.WriteLine($"Processing: {context.DescriptionGroups.ToString}");


                        return Task.CompletedTask;
                    }
                );
            }
        );

        services.ConfigureCors();
        services.ConfigureLogging();
        services.ConfigureSignalR();

        services.ConfigureSettings(configuration);
        services.ConfigureApplicationServices();
        services.ConfigurePersistence(configuration);
        services.ConfigureInfrastructure(configuration);

        return services;
    }

    private static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
    }

    private static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(
            options => options.AddPolicy(
                "CorsPolicy",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader())
        );
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettings<JwtSettings>(configuration);
        services.AddSettings<FrontEndSettings>(configuration);
        services.AddSettings<GmailingAppSettings>(configuration);
        services.AddSettings<TokenHashingSettings>(configuration);
        services.AddSettings<EmailValidationSettings>(configuration);
        services.AddSettings<TokenEncryptionSettings>(configuration);
    }

    private static void AddSettings<T>(this IServiceCollection services, IConfiguration configuration) where T : class, ISettings
    {
        services.Configure<T>(configuration.GetSection(T.SectionName));
    }

    private static void ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging();
    }

    private static void ConfigureSignalR(this IServiceCollection services)
    {
        services.AddSignalR();
    }

    private static void ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
    }

    private static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);
    }

    private static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistenceServices(configuration);
    }
}
