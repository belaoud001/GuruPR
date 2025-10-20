using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using GuruPR.Domain.Entities;
using GuruPR.Persistence.Contexts;
using GuruPR.Persistence.Repositories;
using GuruPR.Application.Settings.Database;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Application.Services.Account.IdentityValidators;

namespace GuruPR.Persistence.Configuration;

public static class ServiceExtensions
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration Configuration)
    {
        var cosmosDBConfig = Configuration.GetSection(CosmosSettings.SectionName)
                                          .Get<CosmosSettings>();

        if (cosmosDBConfig is null)
        {
            throw new ArgumentNullException(nameof(Configuration), "CosmosDBConfig section is missing in configuration.");
        }

        services.AddDbContext<GuruDbContext>(optionsBuilder => optionsBuilder.UseCosmos(accountEndpoint: cosmosDBConfig.AccountEndpoint,
                                                                                        accountKey: cosmosDBConfig.AccountKey,
                                                                                        databaseName: cosmosDBConfig.DatabaseName,
                                                                                        cosmosOptionsAction: cosmosOptions =>
                                                                                        {
#if DEBUG
                                                                                            cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Direct);
#endif
                                                                                        }
                                                                                        ));

        var postgresConfig = Configuration.GetSection(PostgresSettings.SectionName)
                                          .Get<PostgresSettings>();

        if (postgresConfig is null)
        {
            throw new ArgumentNullException(nameof(Configuration), "PostgresConfig section is missing in configuration.");
        }
        
        services.AddDbContext<UserManagementDbContext>(optionBuilder => optionBuilder.UseNpgsql(connectionString: postgresConfig.ConnectionString));

        services.AddIdentity();
        services.AddRepositories();
    }

    private static void AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(
                    identityOptions =>
                    {
                        // Password settings
                        identityOptions.Password.RequiredLength = 8;
                        identityOptions.Password.RequireDigit = true;
                        identityOptions.Password.RequireLowercase = true;
                        identityOptions.Password.RequireUppercase = true;
                        identityOptions.Password.RequireNonAlphanumeric = true;

                        // Email settings
                        identityOptions.User.RequireUniqueEmail = true;
                        identityOptions.SignIn.RequireConfirmedEmail = true;

                        // Lockout settings
                        identityOptions.Lockout.MaxFailedAccessAttempts = 5;
                    }
                )
                .AddUserValidator<StrictEmailDomainValidator>()
                .AddUserValidator<UserProfileValidator>()
                .AddEntityFrameworkStores<UserManagementDbContext>()
                .AddDefaultTokenProviders();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProviderRepository, ProviderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
