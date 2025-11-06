namespace GuruPR.Domain.Entities.Conversation;

public class ConversationAction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Guid ConversationId { get; set; }

    public string ActionType { get; set; } = null!;
}
