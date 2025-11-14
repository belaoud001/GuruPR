using FluentValidation;

using GuruPR.Application.Features.Conversations.Validators.Extensions;

namespace GuruPR.Application.Features.Conversations.Commands.UpdateConversation;

public class UpdateConversationValidator : AbstractValidator<UpdateConversationCommand>
{
    public UpdateConversationValidator()
    {
        RuleFor(conversation => conversation.AgentId).ValidConversationAgentId();

        RuleFor(conversation => conversation.Title).ValidConversationTitle();
    }
}
