using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Profiles.OAuth;
using GuruPR.Application.Services.OAuth;

using Microsoft.Extensions.DependencyInjection;

namespace GuruPR.Application.Settings.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(config => {
            config.AllowNullCollections = true;
            
            config.AddProfile<ProviderProfile>();
            config.AddProfile<ProviderConnectionProfile>();
        });
        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IProviderConnectionService, ProviderConnectionService>();
    }
}
