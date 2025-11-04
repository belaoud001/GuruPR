using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Profiles.Agents;
using GuruPR.Application.Profiles.OAuth;
using GuruPR.Application.Services;
using GuruPR.Application.Services.Account;
using GuruPR.Application.Services.Email;
using GuruPR.Application.Services.OAuth;
using GuruPR.Application.Services.Validators;

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
        });

        services.AddScoped<IUrlValidator, UrlValidator>();
        services.AddScoped<IAccountLinkGenerator, AccountLinkGenerator>();

        services.AddScoped<IEmailTemplateService, EmailTemplateService>();

        services.AddScoped<IToolService, ToolService>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IConversationService, ConversationService>();

        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IProviderConnectionService, ProviderConnectionService>();
    }
}
