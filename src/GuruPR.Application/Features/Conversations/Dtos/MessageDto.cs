using GuruPR.Domain.Entities.Message;
using GuruPR.Domain.Entities.Tool;

namespace GuruPR.Application.Features.Conversations.Dtos;

public class MessageDto
{
    public string ConversationId { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Content { get; set; } = null!;

    public List<ToolCall>? ToolCalls { get; set; }

    public DateTime CreatedAt { get; set; }

    public MessageMetadata MetaData { get; set; } = null!;
}
