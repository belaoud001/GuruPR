using JetBrains.Annotations;

namespace GuruPR.Application.Common.Settings.ModelConfiguration.AzureOpenAI;

public class AzureOpenAIModel
{
    public required string ModelName { get; [UsedImplicitly] init; }

    public required string Endpoint { get; [UsedImplicitly] init; }
}
