using GuruPR.Domain.Entities.Tool;

namespace GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;

public class AgentExecutionResult
{
    public string AgentName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string ModelId { get; set; } = null!;

    public int InputTokens { get; set; }

    public int OutputTokens { get; set; }

    public int TotalTokens { get; set; }

    public List<ToolCall> ToolCalls { get; set; } = new List<ToolCall>();

    public TimeSpan ProcessingTime { get; set; }
}
