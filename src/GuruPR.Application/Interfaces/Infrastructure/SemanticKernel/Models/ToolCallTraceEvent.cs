namespace GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;

public record ToolCallTraceEvent
(
    string PluginName,
    string FunctionName,
    string ArgumentsJson,
    string? OutputJson
);
