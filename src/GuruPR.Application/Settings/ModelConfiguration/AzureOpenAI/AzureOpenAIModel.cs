using JetBrains.Annotations;

namespace GuruPR.Application.Settings.ModelConfiguration.AzureOpenAI;

public class AzureOpenAIModel
{
    public required string DeploymentName { get; [UsedImplicitly] init; }

    public required string Endpoint { get; [UsedImplicitly] init; }

    public required string ApiVersion { get; [UsedImplicitly] init; }
}
