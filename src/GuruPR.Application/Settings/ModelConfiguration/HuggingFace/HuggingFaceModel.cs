using JetBrains.Annotations;

namespace GuruPR.Application.Settings.ModelConfiguration.HuggingFace;

public class HuggingFaceModel
{
    public required string ModelName { get; [UsedImplicitly] init; }

    public required string Endpoint { get; [UsedImplicitly] init; }
}
