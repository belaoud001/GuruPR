using FluentValidation;

using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Agents.Extensions;

namespace GuruPR.Application.Features.Conversations.Validators.Extensions;

public static class ConversationValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidConversationAgentId<T>(this IRuleBuilder<T, string> ruleBuilder, IAgentRepository agentRepository)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Agent is required.")
                          .MustAsync(async (agentId, cancellationToken) =>
                          {
                              try
                              {
                                  await agentRepository.GetByIdOrThrowAsync(agentId, cancellationToken);

                                  return true;
                              }
                              catch
                              {
                                  return false;
                              }
                          })
                          .WithMessage("Agent does not exist.");

    }

    public static IRuleBuilderOptions<T, string> ValidConversationTitle<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Conversation title is required.");

    }
}
