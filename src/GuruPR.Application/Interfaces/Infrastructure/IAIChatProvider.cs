using GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;
using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Infrastructure;

public interface IAIChatProvider
{
    Task<AgentExecutionResult> ExecuteAsync(Agent agent, Conversation conversation, IList<Message> messages, string userMessage);

    Task<string?> GenerateSummaryAsync(IList<Message> messages, string? existingSummary);
}
