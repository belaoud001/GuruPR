using GuruPR.Domain.Entities.Conversation.Operations;
using GuruPR.Domain.Interfaces.Markers;

namespace GuruPR.Domain.Entities.Conversation;

public class Conversation : IOwnedEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserId { get; set; } = null!;

    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ConversationMetadata? Metadata { get; set; } = new ConversationMetadata();

    public void Update(ConversationUpdateData conversationUpdateData)
    {
        Title = conversationUpdateData.Title;
        AgentId = conversationUpdateData.AgentId;

        UpdatedAt = DateTime.UtcNow;
    }
}
