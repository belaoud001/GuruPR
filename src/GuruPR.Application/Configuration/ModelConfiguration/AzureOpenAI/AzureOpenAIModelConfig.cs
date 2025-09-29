using JetBrains.Annotations;

namespace GuruPR.Application.Configuration.ModelConfiguration.AzureOpenAI;

public class AzureOpenAIModelConfig
{
    public required string DeploymentName { get; [UsedImplicitly] init; }

    public required string Endpoint { get; [UsedImplicitly] init; }

    public required string ApiVersion { get; [UsedImplicitly] init; }
}
