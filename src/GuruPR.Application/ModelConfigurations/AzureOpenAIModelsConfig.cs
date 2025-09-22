namespace GuruPR.Application.ModelConfigurations;

public class AzureOpenAIModelsConfig
{
    public required string ApiKey { get; init; }

    public required List<AzureOpenAIModelConfig> AzureOpenAIModels { get; init; }
}
