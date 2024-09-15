using GuruPR.Configuration;
using GuruPR.Hubs;
using GuruPR.Middlewares;
    
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
        services.ConfigureAuthentication();
        services.ConfiureAuthorization();   
        services.ConfigureCors();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureLogging();
        services.ConfigureSignalR();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(
                endpoint =>
                {
                    endpoint.MapControllers();
                    endpoint.MapHub<ChatHub>("/hubs");
                }
        );

        app.UseDefaultFiles();
        app.UseStaticFiles();
    }
}
