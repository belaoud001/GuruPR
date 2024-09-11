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
}
