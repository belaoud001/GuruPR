using GuruPR.Extensions;

namespace GuruPR;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureAppConfiguration();
            builder.Services.ConfigureAllApplicationServices(builder.Configuration);

            var app = builder.Build();

            app.ConfigureMiddlewarePipeline(builder.Environment);
            app.ConfigureEndpoints(builder.Environment);

            await app.InitializeApplicationAsync();

            await app.RunAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}