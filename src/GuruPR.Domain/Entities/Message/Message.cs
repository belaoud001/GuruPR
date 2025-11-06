using GuruPR.Domain.Entities.Tool;

namespace GuruPR.Domain.Entities.Message;

public class Message
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string ConversationId { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Content { get; set; } = null!;

    public List<ToolCall>? ToolCalls { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public MessageMetadata MetaData { get; set; } = null!;
}
