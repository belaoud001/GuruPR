
using System.Text.Json;

using GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace GuruPR.Infrastructure.SemanticKernel.Filters;

internal class FunctionCallTracerFilter : IFunctionInvocationFilter
{
    private readonly ILogger<FunctionCallTracerFilter> _logger;

    public FunctionCallTracerFilter(ILogger<FunctionCallTracerFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var argsJson = JsonSerializer.Serialize(context.Arguments);

        try
        {
            await next(context); // EXECUTE TOOL

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing tool: {Plugin}.{Func}", context.Function.PluginName, context.Function.Name);
            throw;
        }

        var outputJson = JsonSerializer.Serialize(context.Result?.GetValue<object>());

        // Retrieve buffer for this kernel
        if (context.Kernel.Data.TryGetValue(nameof(ToolTraceBuffer), out var buffer))
        {
            if (buffer is not ToolTraceBuffer)
            {
                _logger.LogWarning("ToolTraceBuffer found in kernel data is of incorrect type.");

                return;
            }

            ((ToolTraceBuffer)buffer).Events.Add(new ToolCallTraceEvent(
                context.Function.PluginName ?? "Undefined Plugin Name",
                context.Function.Name ?? "Undefined Function Name",
                argsJson,
                outputJson
            ));
        }

        _logger.LogInformation("Tool executed: {Plugin}.{Func}", context.Function.PluginName, context.Function.Name);
    }
}
