namespace GuruPR.Domain.Entities;

public class Conversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;

    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public Dictionary<string, object> State { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; }

    public ConversationMetadata Metadata { get; set; } = null!;
}

public class ConversationMetadata
{
    public string Summary { get; set; } = string.Empty;

    public int TotalMessages { get; set; } = 0;

    public int TotalTokens { get; set; } = 0;

    public IDictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
}