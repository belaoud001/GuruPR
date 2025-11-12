using FluentValidation;

namespace GuruPR.Application.Features.Conversations.Validators.Extensions;

public static class ConversationValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidConversationAgentId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Agent is required.");

    }

    public static IRuleBuilderOptions<T, string> ValidConversationTitle<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Conversation title is required.");

    }
}
