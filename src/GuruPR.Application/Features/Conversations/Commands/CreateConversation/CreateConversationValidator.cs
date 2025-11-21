using FluentValidation;

using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Conversations.Validators.Extensions;

namespace GuruPR.Application.Features.Conversations.Commands.CreateConversation;

public class CreateConversationValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationValidator(IAgentRepository agentRepository)
    {
        RuleFor(conversation => conversation.AgentId).ValidConversationAgentId(agentRepository);

        RuleFor(conversation => conversation.Title).ValidConversationTitle();
    }
}
