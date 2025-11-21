using GuruPR.Domain.Entities.Agents.Enums;
using GuruPR.Domain.Errors;

namespace GuruPR.Domain.Entities.Agents.Configurations;

public class MemoryConfiguration
{
    public MemoryType Type { get; set; } = MemoryType.ShortTerm;

    public int MaxContextMessages { get; set; } = 50;

    public int MaxContextTokens { get; set; } = 8000;

    public bool UseSemanticMemory { get; set; } = false;

    public string? MemoryCollectionName { get; set; }

    public double RelevanceThreshold { get; set; } = 0.7;

    public int MaxRelevantMemories { get; set; } = 5;

    public bool EnableSummary { get; set; } = false;

    public int SummaryThresholdMessages { get; set; } = 20;

    // Validation Method
    public IEnumerable<ValidationError> Validate()
    {
        var errors = new List<ValidationError>();

        if (MaxContextMessages <= 0)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(MaxContextMessages),
                Message = "MaxContextMessages must be greater than 0."
            });
        }

        if (MaxContextTokens <= 0)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(MaxContextTokens),
                Message = "MaxContextTokens must be greater than 0."
            });
        }

        if (UseSemanticMemory && string.IsNullOrWhiteSpace(MemoryCollectionName))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(MemoryCollectionName),
                Message = "MemoryCollectionName is required when UseSemanticMemory is enabled."
            });
        }

        if (RelevanceThreshold < 0 || RelevanceThreshold > 1)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(RelevanceThreshold),
                Message = "RelevanceThreshold must be between 0 and 1."
            });
        }

        if (MaxRelevantMemories <= 0 || MaxRelevantMemories > 50)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(MaxRelevantMemories),
                Message = "MaxRelevantMemories must be between 1 and 50."
            });
        }

        if (EnableSummary && SummaryThresholdMessages <= 0)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(SummaryThresholdMessages),
                Message = "SummaryThresholdMessages must be greater than 0 when summary is enabled."
            });
        }

        if (EnableSummary && SummaryThresholdMessages > MaxContextMessages)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(SummaryThresholdMessages),
                Message = "SummaryThresholdMessages cannot exceed MaxContextMessages."
            });
        }

        return errors;
    }

}
