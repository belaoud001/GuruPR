using GuruPR.Application.Features.Conversations.Models.CreateCompletion;
using GuruPR.Domain.Entities.Agents;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Entities.Message;

namespace GuruPR.Application.Common.Interfaces.Infrastructure;

public interface IAIChatProvider
{
    Task<AgentExecutionResult> ExecuteAsync(Agent agent, Conversation conversation, IList<Message> messages, string userMessage, CancellationToken cancellationToken = default);

    Task<string?> GenerateSummaryAsync(IList<Message> messages, string? existingSummary, CancellationToken cancellationToken = default);
}
