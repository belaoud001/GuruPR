using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

using GuruPR.Persistence.Contexts;
using GuruPR.Persistence.Repositories;
using GuruPR.Application.Configuration.Database;
using GuruPR.Application.Interfaces.Persistence;

namespace GuruPR.Persistence.Configuration;

public static class ServiceExtensions
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration Configuration)
    {
        var cosmosDBConfig = Configuration.GetSection("CosmosDBConfig")
                                          .Get<CosmosDBConfig>();

        if (cosmosDBConfig is null)
        {
            throw new ArgumentNullException(nameof(Configuration), "CosmosDBConfig section is missing in configuration.");
        }

        services.AddDbContext<GuruDBContext>(optionsBuilder => optionsBuilder.UseCosmos(accountEndpoint: cosmosDBConfig.AccountEndpoint,
                                                                                        accountKey: cosmosDBConfig.AccountKey,
                                                                                        databaseName: cosmosDBConfig.DatabaseName,
                                                                                        cosmosOptionsAction: cosmosOptions =>
                                                                                        {
#if DEBUG
                                                                                            cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Direct);
#endif
                                                                                        }
                                                                                        ));
        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProviderRepository, ProviderRepository>();
    }
}
