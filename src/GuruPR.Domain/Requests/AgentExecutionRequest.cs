namespace GuruPR.Domain.Requests;

public class AgentExecutionRequest
{
    public string AgentId { get; set; } = null!;

    public string ConversationId { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool StreamResponse { get; set; }
}
