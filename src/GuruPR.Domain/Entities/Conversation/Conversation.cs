namespace GuruPR.Domain.Entities.Conversation;

public class Conversation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;

    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public Dictionary<string, object> State { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ConversationMetadata Metadata { get; set; } = null!;
}
