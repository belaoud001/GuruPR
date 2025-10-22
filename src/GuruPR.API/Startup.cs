using GuruPR.Configuration;
using GuruPR.Hubs;
using GuruPR.Middlewares;

using Scalar.AspNetCore;

namespace GuruPR;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureCors();
        services.AddOpenApi();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureLogging();
        services.ConfigureSignalR();
        services.ConfigureSettings(Configuration);
        services.ConfigureApplicationServices();
        services.ConfigurePersistence(Configuration);
        services.ConfigureInfrastructure(Configuration);
    }

    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(
                endpoint =>
                {
                    endpoint.MapControllers();
                    endpoint.MapHub<ChatHub>("/hubs");

                    if (env.IsDevelopment())
                    {
                        endpoint.MapOpenApi();
                        endpoint.MapScalarApiReference();
                    }
                }
        );

        await app.UseIdentityInitializationAsync();
    }
}
