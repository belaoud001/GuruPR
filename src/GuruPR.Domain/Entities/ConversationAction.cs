namespace GuruPR.Domain.Entities;

public class ConversationAction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Guid ConversationId { get; set; }

    public string ActionType { get; set; } = null!;
}
