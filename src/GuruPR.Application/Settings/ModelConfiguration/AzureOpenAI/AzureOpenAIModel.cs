using JetBrains.Annotations;

namespace GuruPR.Application.Settings.ModelConfiguration.AzureOpenAI;

public class AzureOpenAIModel
{
    public required string ModelName { get; [UsedImplicitly] init; }

    public required string Endpoint { get; [UsedImplicitly] init; }

    public required string ApiVersion { get; [UsedImplicitly] init; }
}
