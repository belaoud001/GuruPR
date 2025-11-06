namespace GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;

public class ToolTraceBuffer
{
    public List<ToolCallTraceEvent> Events { get; } = new();
}
