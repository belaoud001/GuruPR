using System.IdentityModel.Tokens.Jwt;

using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Common.Interfaces.Infrastructure.SemanticKernel.Plugins;
using GuruPR.Application.Settings.Authentication;
using GuruPR.Application.Settings.ModelConfiguration.AzureOpenAI;
using GuruPR.Application.Settings.ModelConfiguration.HuggingFace;
using GuruPR.Application.Settings.Security;
using GuruPR.Infrastructure.HttpClients.Spotify;
using GuruPR.Infrastructure.Identity.Constants;
using GuruPR.Infrastructure.SemanticKernel.Filters;
using GuruPR.Infrastructure.SemanticKernel.Plugins;
using GuruPR.Infrastructure.SemanticKernel.Services;
using GuruPR.Infrastructure.Services.Authentication;
using GuruPR.Infrastructure.Services.Email;
using GuruPR.Infrastructure.Services.Security;
using GuruPR.Infrastructure.Services.ThirdParties;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SemanticKernel;

using static System.Text.Encoding;

namespace GuruPR.Infrastructure.Extensions;

public static class ServiceExtensions
{
    #region Public Methods

    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomAuthentication(configuration);
        services.AddTransient<IEmailSender, GmailSender>();
        services.AddScoped<IAIChatProvider, SemanticKernelChatProvider>();
        services.AddSemanticKernel(configuration);
        services.AddSecurity(configuration);
        services.AddHttpClients();
        services.AddThirdPartyServices();
    }

    #endregion

    #region Private Methods

    private static void AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                )
                .AddCookie()
                .AddGoogle(
                    options =>
                    {
                        var externalAuthentication = configuration.GetSection(ExternalAuthentication.SectionName)
                                                                  .Get<ExternalAuthentication>();
                        if (externalAuthentication == null)
                        {
                            throw new InvalidOperationException("External authentication settings are not configured properly.");
                        }

                        var googleSettings = externalAuthentication.Google;

                        options.ClientId = googleSettings.ClientId;
                        options.ClientSecret = googleSettings.ClientSecret;
                        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    }
                )
                .AddJwtBearer(
                    options =>
                    {
                        var jwtSettings = configuration.GetSection(JwtSettings.SectionName)
                                                       .Get<JwtSettings>();

                        if (jwtSettings == null)
                        {
                            throw new InvalidOperationException("JWT settings are not configured properly.");
                        }

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ClockSkew = TimeSpan.Zero,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(UTF8.GetBytes(jwtSettings.SecretKey)),
                            RoleClaimType = JwtClaimTypes.Role,
                            NameClaimType = JwtClaimTypes.Name
                        };


                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                                {
                                    if (context.Request.Cookies.TryGetValue("AccessToken", out var cookieToken))
                                    {
                                        context.Token = cookieToken;
                                    }
                                }

                                return Task.CompletedTask;
                            }
                        };
                    }
                );

        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
    }

    private static void AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
    {
        var kernelBuilder = Kernel.CreateBuilder();

        AddHuggingFaceModels(configuration, kernelBuilder);
        AddAzureOpenAiModels(configuration, kernelBuilder);

        services.AddScoped<IFunctionInvocationFilter, FunctionCallTracerFilter>();
        services.AddScoped<IAgentTool, SpotifyPlugin>();

        services.AddScoped(_ =>
        {
            kernelBuilder.CopyApplicationServices(services);

            return kernelBuilder.Build();
        });
    }

    private static void CopyApplicationServices(this IKernelBuilder kernelBuilder, IServiceCollection services)
    {
        foreach (var serviceDescriptor in services)
        {
            kernelBuilder.Services.Add(serviceDescriptor);
        }
    }

    private static void AddHuggingFaceModels(IConfiguration configuration, IKernelBuilder kernelBuilder)
    {
        var huggingFaceModelsConfig = configuration.GetSection(HuggingFaceModels.SectionName)
                                                   .Get<HuggingFaceModels>();

        if (huggingFaceModelsConfig == null ||
            huggingFaceModelsConfig.HuggingFaceModelCollection?.Count <= 0 ||
            huggingFaceModelsConfig.ApiKey == null)
        {
            throw new InvalidOperationException("HuggingFaceModelsConfig is missing or contains no models.");
        }

        var huggingFaceModels = huggingFaceModelsConfig?.HuggingFaceModelCollection;
        foreach (var huggingFaceModel in huggingFaceModels ?? [])
        {
            kernelBuilder.Services.AddHuggingFaceChatCompletion(
                serviceId: huggingFaceModel.ModelName,
                model: huggingFaceModel.ModelName,
                apiKey: huggingFaceModelsConfig?.ApiKey,
                endpoint: new Uri(huggingFaceModel.Endpoint)
            );
        }
    }

    private static void AddAzureOpenAiModels(IConfiguration configuration, IKernelBuilder kernelBuilder)
    {
        var azureOpenAiModelsConfig = configuration.GetSection(AzureOpenAIModels.SectionName)
                                                   .Get<AzureOpenAIModels>();

        if (azureOpenAiModelsConfig == null ||
            azureOpenAiModelsConfig.AzureOpenAIModelCollection?.Count <= 0 ||
            string.IsNullOrEmpty(azureOpenAiModelsConfig.ApiKey))
        {
            throw new InvalidOperationException("AzureOpenAIConfig is missing or contains no models.");
        }

        var azureOpenAiModels = azureOpenAiModelsConfig?.AzureOpenAIModelCollection;
        foreach (var azureOpenAiModel in azureOpenAiModels ?? [])
        {
            kernelBuilder.Services.AddAzureOpenAIChatCompletion(
                serviceId: azureOpenAiModel.ModelName,
                deploymentName: azureOpenAiModel.ModelName,
                endpoint: azureOpenAiModel.Endpoint,
                apiKey: azureOpenAiModelsConfig!.ApiKey
            );
        }
    }

    private static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenEncryptionConfig = configuration.GetSection(TokenEncryptionSettings.SectionName)
                                                 .Get<TokenEncryptionSettings>()
                                                 ?? throw new InvalidOperationException("TokenEncryptionConfig section is missing in configuration.");

        if (string.IsNullOrEmpty(tokenEncryptionConfig.Key))
        {
            throw new InvalidOperationException("TokenEncryptionConfig or its Key is missing in configuration.");
        }

        services.AddSingleton<ITokenEncryptionService, TokenEncryptionService>();
        services.AddSingleton<IHasher, HmacTokenHasher>();
    }

    private static void AddHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient<SpotifyOAuthClient>(httpClient =>
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<SpotifyClient>(httpClient =>
        {
            httpClient.BaseAddress = new Uri("https://api.spotify.com/");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        });
    }

    private static void AddThirdPartyServices(this IServiceCollection services)
    {
        services.AddScoped<ISpotifyService, SpotifyService>();
    }

    #endregion
}
