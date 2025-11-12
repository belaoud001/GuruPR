using FluentValidation;

using GuruPR.Application.Features.Conversations.Validators.Extensions;

namespace GuruPR.Application.Features.Conversations.Commands.UpdateConversation;

public class UpdateConversationCommandValidator : AbstractValidator<UpdateConversationCommand>
{
    public UpdateConversationCommandValidator()
    {
        RuleFor(conversation => conversation.AgentId).ValidConversationAgentId();

        RuleFor(conversation => conversation.Title).ValidConversationTitle();
    }
}
