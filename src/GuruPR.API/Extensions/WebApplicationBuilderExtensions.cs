namespace GuruPR.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder ConfigureAppConfiguration(this WebApplicationBuilder builder)
    {
        var env = builder.Environment;

        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                             .AddEnvironmentVariables();

        if (env.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        return builder;
    }
}
