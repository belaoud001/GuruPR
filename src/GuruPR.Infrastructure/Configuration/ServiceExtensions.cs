using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using GuruPR.Application.Configuration.ModelConfiguration.AzureOpenAI;
using GuruPR.Application.Configuration.ModelConfiguration.HuggingFace;

namespace GuruPR.Infrastructure.Configuration;

#pragma warning disable SKEXP0070 // Suppress experimental feature warning

public static class ServiceExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSemanticKernel(configuration);
    }

    private static void AddSemanticKernel(this IServiceCollection services, IConfiguration configuration)
    {
        var kernelBuilder = Kernel.CreateBuilder();

        AddHuggingFaceModels(configuration, kernelBuilder);
        AddAzureOpenAiModels(configuration, kernelBuilder);

        var kernel = kernelBuilder.Build();

        services.AddSingleton<Kernel>(kernel);
    }

    private static void AddHuggingFaceModels(IConfiguration configuration, IKernelBuilder kernelBuilder)
    {
        var huggingFaceModelsConfig = configuration.GetSection("HuggingFaceModelsConfig")
                                                   .Get<HuggingFaceModelsConfig>();

        if (huggingFaceModelsConfig == null ||
            huggingFaceModelsConfig.HuggingFaceModels?.Count <= 0 ||
            huggingFaceModelsConfig.ApiKey == null)
        {
            throw new InvalidOperationException("HuggingFaceModelsConfig is missing or contains no models.");
        }

        var huggingFaceModels = huggingFaceModelsConfig?.HuggingFaceModels;
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
        var azureOpenAiModelsConfig = configuration.GetSection("AzureOpenAIModelsConfig")
                                                   .Get<AzureOpenAIModelsConfig>();

        if (azureOpenAiModelsConfig == null ||
            azureOpenAiModelsConfig.AzureOpenAIModels?.Count <= 0 ||
            string.IsNullOrEmpty(azureOpenAiModelsConfig.ApiKey))
        {
            throw new InvalidOperationException("AzureOpenAIConfig is missing or contains no models.");
        }

        var azureOpenAiModels = azureOpenAiModelsConfig?.AzureOpenAIModels;
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
}

#pragma warning restore SKEXP0070 // Suppress experimental feature warning