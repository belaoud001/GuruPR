namespace GuruPR.Domain.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ConversationId { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Content { get; set; } = null!;

    public List<ToolCall>? ToolCalls { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MessageMetadata MetaData { get; set; } = null!;
}

public class ToolCall
{
    public string Name { get; set; } = null!;

    public string PluginName { get; set; } = null!;

    public string Arguments { get; set; } = null!;

    public string Output { get; set; } = null!;
}

public class MessageMetadata
{
    public string AgentName { get; set; } = null!;

    public int TokenCount { get; set; }

    public double? Cost { get; set; }

    public TimeSpan? ProcessingTime { get; set; }

    public string? ModelUsed { get; set; }

    public IDictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
}