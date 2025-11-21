using FluentValidation;

using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Conversations.Validators.Extensions;

namespace GuruPR.Application.Features.Conversations.Commands.UpdateConversation;

public class UpdateConversationValidator : AbstractValidator<UpdateConversationCommand>
{
    public UpdateConversationValidator(IAgentRepository agentRepository)
    {
        RuleFor(conversation => conversation.AgentId).ValidConversationAgentId(agentRepository);

        RuleFor(conversation => conversation.Title).ValidConversationTitle();
    }
}
