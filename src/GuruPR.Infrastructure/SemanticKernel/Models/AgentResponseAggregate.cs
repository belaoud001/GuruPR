namespace GuruPR.Infrastructure.SemanticKernel.Models;
public class AgentResponseAggregate
{
    public List<string> Messages { get; } = new();

    public string? ModelId { get; set; }

    public object? LastInnerContent { get; set; }
}
