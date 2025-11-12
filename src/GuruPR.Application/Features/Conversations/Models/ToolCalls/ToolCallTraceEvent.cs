namespace GuruPR.Application.Features.Conversations.Models.ToolCalls;

public record ToolCallTraceEvent
(
    string PluginName,
    string FunctionName,
    string ArgumentsJson,
    string? OutputJson
);
