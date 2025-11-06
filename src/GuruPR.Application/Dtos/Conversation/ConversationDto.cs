using GuruPR.Domain.Entities.Conversation;

namespace GuruPR.Application.Dtos.Conversation;

public class ConversationDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;

    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public Dictionary<string, object> State { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    public ConversationMetadata Metadata { get; set; } = null!;
}
