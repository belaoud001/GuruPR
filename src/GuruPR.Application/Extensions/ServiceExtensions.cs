using FluentValidation;

using GuruPR.Application.Common.Behaviors;
using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Common.Markers;
using GuruPR.Application.Common.Services;
using GuruPR.Application.Features.Account.Commands.ConfirmEmail;
using GuruPR.Application.Features.Account.Commands.ExternalLogin.Google.GoogleCallback;
using GuruPR.Application.Features.Account.Commands.ExternalLogin.Google.GoogleLogin;
using GuruPR.Application.Features.Account.Commands.Login;
using GuruPR.Application.Features.Account.Commands.Logout;
using GuruPR.Application.Features.Account.Commands.Register;
using GuruPR.Application.Features.Account.Services;
using GuruPR.Application.Features.Agents.Commands.CreateAgent;
using GuruPR.Application.Features.Agents.Commands.UpdateAgent;
using GuruPR.Application.Features.Conversations.Commands.CreateConversation;
using GuruPR.Application.Features.ProviderConnections.Commands.CreateProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Commands.DeleteProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Commands.UpdateProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnectionById;
using GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnections;
using GuruPR.Application.Features.Providers.Commands.CreateProvider;
using GuruPR.Application.Features.Providers.Commands.DeleteProvider;
using GuruPR.Application.Features.Providers.Commands.UpdateProvider;
using GuruPR.Application.Features.Providers.Queries.GetProviderById;
using GuruPR.Application.Features.Providers.Queries.GetProviders;
using GuruPR.Application.Profiles.Agents;
using GuruPR.Application.Profiles.Conversations;
using GuruPR.Application.Profiles.OAuth;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace GuruPR.Application.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AllowNullCollections = true;

            config.AddProfile<ProviderProfile>();
            config.AddProfile<ProviderConnectionProfile>();
            config.AddProfile<AgentProfile>();
            config.AddProfile<ConversationProfile>();
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UserContextEnrichmentBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OwnershipValidatorBehavior<,,>));

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(ApplicationMarker).Assembly);
        });

        services.AddFluentValidators();

        services.AddScoped<IUrlValidator, UrlValidator>();
        services.AddScoped<IAccountLinkGenerator, AccountLinkGenerator>();

        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
    }

    private static void AddFluentValidators(this IServiceCollection services)
    {
        // Agent Validators
        services.AddScoped<IValidator<CreateAgentCommand>, CreateAgentValidator>();
        services.AddScoped<IValidator<UpdateAgentCommand>, UpdateAgentValidator>();

        // Conversation Validators
        services.AddScoped<IValidator<CreateConversationCommand>, CreateConversationValidator>();
        services.AddScoped<IValidator<UpdateAgentCommand>, UpdateAgentValidator>();

        // Account Validators
        services.AddScoped<IValidator<RegisterCommand>, RegisterValidator>();
        services.AddScoped<IValidator<LoginCommand>, LoginValidator>();
        services.AddScoped<IValidator<LogoutCommand>, LogoutValidator>();
        services.AddScoped<IValidator<ConfirmEmailCommand>, ConfirmEmailValidator>();
        services.AddScoped<IValidator<GoogleCallbackCommand>, GoogleCallbackValidator>();
        services.AddScoped<IValidator<GoogleLoginCommand>, GoogleLoginValidator>();

        // Provider Connection Validators
        services.AddScoped<IValidator<CreateProviderConnectionCommand>, CreateProviderConnectionValidator>();
        services.AddScoped<IValidator<UpdateProviderConnectionCommand>, UpdateProviderConnectionValidator>();
        services.AddScoped<IValidator<DeleteProviderConnectionCommand>, DeleteProviderConnectionValidator>();

        services.AddScoped<IValidator<GetProviderConnectionByIdQuery>, GetProviderConnectionByIdValidator>();
        services.AddScoped<IValidator<GetProviderConnectionsQuery>, GetProviderConnectionsValidator>();

        // Provider Validators
        services.AddScoped<IValidator<CreateProviderCommand>, CreateProviderValidator>();
        services.AddScoped<IValidator<UpdateProviderCommand>, UpdateProviderValidator>();
        services.AddScoped<IValidator<DeleteProviderCommand>, DeleteProviderValidator>();

        services.AddScoped<IValidator<GetProviderByIdQuery>, GetProviderByIdValidator>();
        services.AddScoped<IValidator<GetProvidersQuery>, GetProvidersValidator>();
    }
}
