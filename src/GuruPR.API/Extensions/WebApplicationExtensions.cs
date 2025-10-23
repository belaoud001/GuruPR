using GuruPR.Configuration;
using GuruPR.Hubs;
using GuruPR.Middlewares;

using Scalar.AspNetCore;

namespace GuruPR.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();
        
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication ConfigureEndpoints(this WebApplication app, IWebHostEnvironment env)
    {
        app.MapControllers();
        app.MapHub<ChatHub>("/hubs");

        if (env.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }

    public static async Task<WebApplication> InitializeApplicationAsync(this WebApplication app)
    {
        await app.UseIdentityInitializationAsync();

        return app;
    }
}
