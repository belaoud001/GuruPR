using JetBrains.Annotations;

namespace GuruPR.Application.Configuration.ModelConfiguration.HuggingFace;

public class HuggingFaceModelConfig
{
    public required string ModelName { get; [UsedImplicitly] init; }

    public required string Endpoint { get; [UsedImplicitly] init; }
}
