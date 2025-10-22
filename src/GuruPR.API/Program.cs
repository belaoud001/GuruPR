using GuruPR;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(ConfigureConfiguration)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

    private static void ConfigureConfiguration(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
    {
        var env = context.HostingEnvironment;

        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();

        if (env.IsDevelopment())
        {
            configurationBuilder.AddUserSecrets<Program>();
        }
    }
}
