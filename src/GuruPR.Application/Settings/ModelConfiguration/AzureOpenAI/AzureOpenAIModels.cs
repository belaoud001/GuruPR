namespace GuruPR.Application.Settings.ModelConfiguration.AzureOpenAI;

public class AzureOpenAIModels
{
    public const string SectionName = "AzureOpenAIModels";

    public required string ApiKey { get; init; }

    public required List<AzureOpenAIModel> AzureOpenAIModelCollection { get; init; }
}
