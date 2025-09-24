namespace GuruPR.Application.Configuration.ModelConfiguration.AzureOpenAI;

public class AzureOpenAIModelsConfig
{
    public required string ApiKey { get; init; }

    public required List<AzureOpenAIModelConfig> AzureOpenAIModels { get; init; }
}
