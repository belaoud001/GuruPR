using FluentValidation;

using GuruPR.Domain.Entities.Agents.Configurations;

namespace GuruPR.Application.Features.Agents.Validators;

public class ModelConfigurationValidator : AbstractValidator<ModelConfiguration>
{
    public ModelConfigurationValidator()
    {
        RuleFor(modelConfiguration => modelConfiguration.Provider).NotEmpty()
                                                                  .WithMessage("Model provider is required.");

        RuleFor(modelConfiguration => modelConfiguration.ModelType).NotEmpty()
                                                                   .WithMessage("Model type is required.");

        RuleFor(modelConfiguration => modelConfiguration.ModelName).NotEmpty()
                                                                   .WithMessage("Model name is required.");

        RuleFor(modelConfiguration => modelConfiguration.Temperature).GreaterThanOrEqualTo(0)
                                                                     .WithMessage("Temperature must be greater than or equal to 0 and preferably less than or equal to 1.");

        RuleFor(modelConfiguration => modelConfiguration.TopP).InclusiveBetween(0, 1)
                                                              .WithMessage("TopP must be between 0 and 1.");

        RuleFor(modelConfiguration => modelConfiguration.MaxTokens).GreaterThan(0)
                                                                   .WithMessage("MaxTokens must be greater than 0.");

        RuleFor(modelConfiguration => modelConfiguration.FrequencyPenalty).InclusiveBetween(0, 2)
                                                                          .WithMessage("FrequencyPenalty must be between 0 and 2.");

        RuleFor(modelConfiguration => modelConfiguration.PresencePenalty).InclusiveBetween(0, 2)
                                                                         .WithMessage("PresencePenalty must be between 0 and 2.");
    }
}
