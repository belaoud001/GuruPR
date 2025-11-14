using FluentValidation;

using GuruPR.Application.Common.Behaviors;
using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Common.Services;
using GuruPR.Application.Features.Account.Commands.ConfirmEmail;
using GuruPR.Application.Features.Account.Commands.ExternalLogin.GoogleCallback;
using GuruPR.Application.Features.Account.Commands.ExternalLogin.GoogleLogin;
using GuruPR.Application.Features.Account.Commands.Login;
using GuruPR.Application.Features.Account.Commands.Logout;
using GuruPR.Application.Features.Account.Commands.Register;
using GuruPR.Application.Features.Account.Services;
using GuruPR.Application.Features.Agents.Commands.CreateAgent;
using GuruPR.Application.Features.Agents.Commands.UpdateAgent;
using GuruPR.Application.Features.Conversations.Commands.CreateConversation;
using GuruPR.Application.Profiles.Agents;
using GuruPR.Application.Profiles.Conversations;
using GuruPR.Application.Profiles.OAuth;
using GuruPR.Application.Services.OAuth;

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

        services.AddFluentValidators();

        services.AddScoped<IUrlValidator, UrlValidator>();
        services.AddScoped<IAccountLinkGenerator, AccountLinkGenerator>();

        services.AddScoped<IEmailTemplateService, EmailTemplateService>();

        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IProviderConnectionService, ProviderConnectionService>();
    }

    private static void AddFluentValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateAgentCommand>, CreateAgentValidator>();
        services.AddScoped<IValidator<UpdateAgentCommand>, UpdateAgentValidator>();

        services.AddScoped<IValidator<CreateConversationCommand>, CreateConversationValidator>();
        services.AddScoped<IValidator<UpdateAgentCommand>, UpdateAgentValidator>();

        services.AddScoped<IValidator<RegisterCommand>, RegisterValidator>();
        services.AddScoped<IValidator<LoginCommand>, LoginValidator>();
        services.AddScoped<IValidator<LogoutCommand>, LogoutValidator>();
        services.AddScoped<IValidator<ConfirmEmailCommand>, ConfirmEmailValidator>();
        services.AddScoped<IValidator<GoogleCallbackCommand>, GoogleCallbackValidator>();
        services.AddScoped<IValidator<GoogleLoginCommand>, GoogleLoginValidator>();
    }
}
