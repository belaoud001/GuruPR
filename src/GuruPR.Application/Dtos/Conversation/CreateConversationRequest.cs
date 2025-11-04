using GuruPR.Domain.Entities.Conversation;

namespace GuruPR.Application.Dtos.Conversation;

public class CreateConversationRequest
{
    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public ConversationMetadata? Metadata { get; set; } = null!;
}
