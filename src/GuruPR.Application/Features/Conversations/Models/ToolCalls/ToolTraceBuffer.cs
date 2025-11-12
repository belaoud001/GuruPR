namespace GuruPR.Application.Features.Conversations.Models.ToolCalls;

public class ToolTraceBuffer
{
    public List<ToolCallTraceEvent> Events { get; } = new();
}
