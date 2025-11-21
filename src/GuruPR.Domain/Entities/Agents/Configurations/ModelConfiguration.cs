namespace GuruPR.Domain.Entities.Agents.Configurations;

public class ModelConfiguration
{
    public string Provider { get; set; } = null!;

    public string ModelType { get; set; } = null!;

    public string ModelName { get; set; } = null!;

    public double Temperature { get; set; } = 0.6;

    public int MaxTokens { get; set; } = 2000;

    public double TopP { get; set; } = 0.9;

    public double FrequencyPenalty { get; set; } = 0.0;

    public double PresencePenalty { get; set; } = 0.0;

    public IList<string> StopSequences { get; set; } = new List<string>();

    public Dictionary<string, object> AdditionalParameters { get; set; } = new Dictionary<string, object>();

    public bool IsReasoningModel() => ModelType?.Contains("Reasoning", StringComparison.OrdinalIgnoreCase) ?? false;
}
