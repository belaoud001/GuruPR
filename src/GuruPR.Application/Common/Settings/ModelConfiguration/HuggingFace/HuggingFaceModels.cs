namespace GuruPR.Application.Common.Settings.ModelConfiguration.HuggingFace;

public class HuggingFaceModels
{
    public const string SectionName = "HuggingFaceModels";

    public required string ApiKey { get; init; }

    public required List<HuggingFaceModel> HuggingFaceModelCollection { get; init; }
}
