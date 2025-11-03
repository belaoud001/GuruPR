namespace GuruPR.Domain.Entities;

public class MessageMetadata
{
    public string AgentName { get; set; } = null!;

    public int TokenCount { get; set; }

    public double? Cost { get; set; }

    public TimeSpan? ProcessingTime { get; set; }

    public string? ModelUsed { get; set; }

    public IDictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
}
