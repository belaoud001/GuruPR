using Microsoft.Extensions.DependencyInjection;

using GuruPR.Application.Profiles;
using GuruPR.Application.Services.OAuth;

namespace GuruPR.Application.Configuration.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(config => {
            config.AddProfile<ProviderProfile>();
        });
        services.AddScoped<ProviderManagementService>();
    }
}
