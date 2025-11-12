using GuruPR.Domain.Entities.Conversation;

namespace GuruPR.Application.Features.Conversations.Dtos;

public class ConversationDto
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ConversationMetadata Metadata { get; set; } = null!;
}
