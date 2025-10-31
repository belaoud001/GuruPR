using GuruPR.Domain.Entities.Configurations.Enums;

namespace GuruPR.Domain.Entities.Configurations;

public class MemoryConfiguration
{
    public MemoryType Type { get; set; } = MemoryType.ShortTerm;

    public int MaxContextMessages { get; set; } = 50;

    public int MaxContextTokens { get; set; } = 8000;

    public bool UseSemanticMemory { get; set; } = false;

    public string? MemoryCollectionName { get; set; }

    public double RelevanceThreshold { get; set; } = 0.7;

    public int MaxRelevantMemories { get; set; } = 5;

    public bool EnableSummary { get; set; } = true;

    public int SummaryThresholdMessages { get; set; } = 20;
}
