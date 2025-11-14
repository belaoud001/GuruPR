using FluentValidation;

using GuruPR.Application.Features.Agents.Validators.Extensions;

namespace GuruPR.Application.Features.Agents.Commands.UpdateAgent;

public class UpdateAgentValidator : AbstractValidator<UpdateAgentCommand>
{
    public UpdateAgentValidator()
    {
        RuleFor(agent => agent.Id).NotEmpty()
                                  .WithMessage("Agent Id is required");

        RuleFor(agent => agent.Name).ValidAgentName();

        RuleFor(agent => agent.AvatarUrl).ValidAvatarUrl();

        RuleFor(agent => agent.Description).ValidDescription();

        RuleFor(agent => agent.ModelConfiguration).ValidModelConfiguration();

        RuleFor(agent => agent.MemoryConfiguration).ValidMemoryConfiguration();
    }
}
