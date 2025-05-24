namespace GuruPR.Application.ModelConfigurations;

public class HuggingFaceModelsConfig
{
    public required string ApiKey { get; init; }
    
    public required List<HuggingFaceModelConfig> HuggingFaceModels { get; init; }
}