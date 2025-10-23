using GuruPR.Persistence.Identity;

namespace GuruPR.Configuration;

public static class ApplicationBuilderExtensions
{
    public async static Task<IApplicationBuilder> UseIdentityInitializationAsync(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var services = scope.ServiceProvider;
            var config = services.GetRequiredService<IConfiguration>();

            await DbInitializer.SeedRolesAndAdminAsync(services, config);
        }

        return app;
    }
}
