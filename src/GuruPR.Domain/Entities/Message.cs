namespace GuruPR.Domain.Entities;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ConversationId { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public IDictionary<string, object> MetaData { get; set; } = null!;
}
