using FluentValidation;

using GuruPR.Application.Features.Agents.Validators.Extensions;

namespace GuruPR.Application.Features.Agents.Commands.CreateAgent;

public class CreateAgentValidator : AbstractValidator<CreateAgentCommand>
{
    public CreateAgentValidator()
    {
        RuleFor(agent => agent.Name).ValidAgentName();

        RuleFor(agent => agent.AvatarUrl).ValidAvatarUrl();

        RuleFor(agent => agent.Description).ValidDescription();

        RuleFor(agent => agent.ModelConfiguration).ValidModelConfiguration();

        RuleFor(agent => agent.MemoryConfiguration).ValidMemoryConfiguration();
    }
}
