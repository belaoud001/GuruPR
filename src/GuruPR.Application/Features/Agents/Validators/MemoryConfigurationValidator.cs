using FluentValidation;

using GuruPR.Domain.Entities.Agents.Configurations;

namespace GuruPR.Application.Features.Agents.Validators;

public class MemoryConfigurationValidator : AbstractValidator<MemoryConfiguration>
{
    public MemoryConfigurationValidator()
    {
        RuleFor(memoryConfiguration => memoryConfiguration.MaxContextMessages).InclusiveBetween(1, 40)
                                                                              .WithErrorCode("MaxContextMessages")
                                                                              .WithMessage("MaxContextMessages must be greater than 0.");

        RuleFor(memoryConfiguration => memoryConfiguration.MaxContextTokens).GreaterThan(0)
                                                                            .WithErrorCode("MaxContextTokens")
                                                                            .WithMessage("MaxContextTokens must be greater than 0.");

        RuleFor(memoryConfiguration => memoryConfiguration.MemoryCollectionName).NotEmpty()
                                                                                .WithErrorCode("MemoryCollectionName")
                                                                                .WithMessage("MemoryCollectionName is required when UseSemanticMemory is enabled.")
                                                                                .When(memoryConfiguration => memoryConfiguration.UseSemanticMemory);

        RuleFor(memoryConfiguration => memoryConfiguration.RelevanceThreshold).InclusiveBetween(0, 1)
                                                                              .WithErrorCode("RelevanceThreshold")
                                                                              .WithMessage("RelevanceThreshold must be between 0 and 1.");

        RuleFor(memoryConfiguration => memoryConfiguration.MaxRelevantMemories).InclusiveBetween(1, 50)
                                                                               .WithErrorCode("MaxRelevantMemories")
                                                                               .WithMessage("MaxRelevantMemories must be between 1 and 50.");

        RuleFor(memoryConfiguration => memoryConfiguration.SummaryThresholdMessages).GreaterThan(0)
                                                                                    .WithErrorCode("SummaryThresholdMessages")
                                                                                    .WithMessage("SummaryThresholdMessages must be greater than 0 when summary is enabled.")
                                                                                    .When(memoryConfiguration => memoryConfiguration.EnableSummary);

        RuleFor(memoryConfiguration => memoryConfiguration.SummaryThresholdMessages).LessThanOrEqualTo(memoryConfiguration => memoryConfiguration.MaxContextMessages)
                                                                                    .WithErrorCode("SummaryThresholdMessages")
                                                                                    .WithMessage("SummaryThresholdMessages cannot exceed MaxContextMessages.")
                                                                                    .When(memoryConfiguration => memoryConfiguration.EnableSummary);
    }
}
