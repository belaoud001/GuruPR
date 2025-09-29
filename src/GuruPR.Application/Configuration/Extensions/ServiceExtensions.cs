using Microsoft.Extensions.DependencyInjection;

using GuruPR.Application.Services.OAuth;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Profiles.OAuth;

namespace GuruPR.Application.Configuration.Extensions;

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
    }
}
