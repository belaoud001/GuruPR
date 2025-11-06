namespace GuruPR.Domain.Entities.Conversation;

public class ConversationMetadata
{
    public string Summary { get; set; } = string.Empty;

    public int TotalMessages { get; set; } = 0;

    public int TotalTokens { get; set; } = 0;

    public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
}
