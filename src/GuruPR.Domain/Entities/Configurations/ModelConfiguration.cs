using GuruPR.Domain.Errors;

namespace GuruPR.Domain.Entities.Configurations;

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

    public bool IsValid() => !Validate().Any();

    public IEnumerable<ValidationError> Validate()
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(Provider))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(Provider),
                Message = "Model provider is required."
            });
        }

        if (string.IsNullOrWhiteSpace(ModelType))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(ModelType),
                Message = "Model type is required."
            });
        }

        if (string.IsNullOrWhiteSpace(ModelName))
        {
            errors.Add(new ValidationError
            {
                Field = nameof(ModelName),
                Message = "Model name is required."
            });
        }

        if (Temperature < 0)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(Temperature),
                Message = "Temperature must be greater than or equal to 0 and preferably less than or equal to 1."
            });
        }

        if (TopP < 0 || TopP > 1)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(TopP),
                Message = "TopP must be between 0 and 1."
            });
        }

        if (MaxTokens <= 0)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(MaxTokens),
                Message = "MaxTokens must be greater than 0."
            });
        }

        if (FrequencyPenalty < 0 || FrequencyPenalty > 2)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(FrequencyPenalty),
                Message = "FrequencyPenalty must be between 0 and 2."
            });
        }

        if (PresencePenalty < 0 || PresencePenalty > 2)
        {
            errors.Add(new ValidationError
            {
                Field = nameof(PresencePenalty),
                Message = "PresencePenalty must be between 0 and 2."
            });
        }

        return errors;
    }

    public bool IsReasoningModel() => ModelType?.Contains("Reasoning", StringComparison.OrdinalIgnoreCase) ?? false;
}
