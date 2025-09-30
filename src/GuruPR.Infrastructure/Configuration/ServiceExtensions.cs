using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using GuruPR.Infrastructure.Services.Security;
using GuruPR.Application.Configuration.Security;
using GuruPR.Infrastructure.Services.ThirdParties;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Settings.ModelConfiguration.HuggingFace;
using GuruPR.Application.Settings.ModelConfiguration.AzureOpenAI;
using GuruPR.Infrastructure.SemanticKernel.Plugins;

namespace GuruPR.Infrastructure.Configuration;

#pragma warning disable SKEXP0070 // Suppress experimental feature warning

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSemanticKernel(configuration);
        services.AddSecurity(configuration);
        services.AddScoped<ISpotifyService, SpotifyService>();
    }

    private static void AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
    {
        var kernelBuilder = Kernel.CreateBuilder();

        AddHuggingFaceModels(configuration, kernelBuilder);
        AddAzureOpenAiModels(configuration, kernelBuilder);

        var kernel = kernelBuilder.Build();
        
        var spotifyPlugin = new SpotifyPlugin(new SpotifyService());
        kernel.Plugins.AddFromObject(spotifyPlugin, "SpotifyPlugin");

        services.AddSingleton<Kernel>(kernel);
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
                serviceId: azureOpenAiModel.DeploymentName,
                deploymentName: azureOpenAiModel.DeploymentName,
                endpoint: azureOpenAiModel.Endpoint,
                apiKey: azureOpenAiModelsConfig!.ApiKey,
                apiVersion: azureOpenAiModel.ApiVersion
            );
        }
    }

    private static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenEncryptionConfig = configuration.GetSection(TokenEncryption.SectionName)
                                                 .Get<TokenEncryption>() 
                                                 ?? throw new InvalidOperationException("TokenEncryptionConfig section is missing in configuration.");

        if (string.IsNullOrEmpty(tokenEncryptionConfig.Key))
        {
            throw new InvalidOperationException("TokenEncryptionConfig or its Key is missing in configuration.");
        }

        services.AddSingleton<ITokenEncryptionService>(_ => new TokenEncryptionService(tokenEncryptionConfig.Key));
    }
}

#pragma warning restore SKEXP0070 // Suppress experimental feature warning