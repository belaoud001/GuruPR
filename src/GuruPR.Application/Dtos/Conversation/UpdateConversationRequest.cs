using GuruPR.Domain.Entities.Conversation;

namespace GuruPR.Application.Dtos.Conversation;

public class UpdateConversationRequest
{
    public string? AgentId { get; set; } = null!;

    public string? Title { get; set; } = null!;

    public Dictionary<string, object>? State { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    public ConversationMetadata? Metadata { get; set; }
}
