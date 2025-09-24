namespace GuruPR.Application.Configuration.ModelConfiguration.HuggingFace;

public class HuggingFaceModelsConfig
{
    public required string ApiKey { get; init; }

    public required List<HuggingFaceModelConfig> HuggingFaceModels { get; init; }
}