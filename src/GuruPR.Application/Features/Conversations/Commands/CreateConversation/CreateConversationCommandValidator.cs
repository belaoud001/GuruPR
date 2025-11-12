using FluentValidation;

using GuruPR.Application.Features.Conversations.Validators.Extensions;

namespace GuruPR.Application.Features.Conversations.Commands.CreateConversation;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(conversation => conversation.AgentId).ValidConversationAgentId();

        RuleFor(conversation => conversation.Title).ValidConversationTitle();
    }
}
