using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using GuruPR.Application.ModelConfigurations;

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
}

#pragma warning restore SKEXP0070 // Suppress experimental feature warning