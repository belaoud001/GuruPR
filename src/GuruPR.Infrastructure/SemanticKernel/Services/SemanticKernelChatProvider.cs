using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;
using GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Plugins;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Entities.Configurations;
using GuruPR.Infrastructure.SemanticKernel.Models;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Agent = GuruPR.Domain.Entities.Agent;

namespace GuruPR.Infrastructure.SemanticKernel.Services;

public class SemanticKernelChatProvider : IAIChatProvider
{
    private readonly ILogger<SemanticKernelChatProvider> _logger;
    private readonly Kernel _kernel;

    public SemanticKernelChatProvider(ILogger<SemanticKernelChatProvider> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
    }

    #region Public Methods

    public async Task<AgentExecutionResult> ExecuteAsync(Agent agent, Conversation conversation, IList<Message> messages, string userMessage)
    {
        var startTime = DateTime.UtcNow;

        var kernel = SetupKernel(agent);
        var chatHistory = PrepareChatHistory(agent, conversation, messages);
        var response = await ExecuteWithKernelAsync(kernel, chatHistory, agent);

        kernel.Data.TryGetValue(nameof(ToolTraceBuffer), out var trace);

        ToolTraceBuffer? toolTraceBuffer = kernel.Data.TryGetValue(nameof(ToolTraceBuffer), out var toolTrace)
                                                       ? toolTrace as ToolTraceBuffer : null;

        var toolCalls = toolTraceBuffer?.Events.Select(evnt => new ToolCall
        {
            Name = evnt.FunctionName,
            PluginName = evnt.PluginName,
            Arguments = evnt.ArgumentsJson,
            Output = evnt.OutputJson ?? string.Empty
        }
                                                       ).ToList() ?? new List<ToolCall>();

        if (toolTraceBuffer == null)
        {
            _logger.LogInformation("No tool calls were made during the agent execution.");
        }

        return new AgentExecutionResult
        {
            AgentName = agent.Name,
            Content = response.Messages.FirstOrDefault() ?? string.Empty,
            ModelId = response.ModelId ?? agent.ModelConfiguration.ModelName,
            InputTokens = GetTokenCount(response.LastInnerContent, Constants.TokenInputCount),
            OutputTokens = GetTokenCount(response.LastInnerContent, Constants.TokenOutputCount),
            TotalTokens = GetTokenCount(response.LastInnerContent, Constants.TokenTotalCount),
            ToolCalls = toolCalls,
            ProcessingTime = DateTime.UtcNow - startTime
        };
    }

    public async Task<string?> GenerateSummaryAsync(IList<Message> messages, string? existingSummary)
    {
        string summaryPrompt;

        if (string.IsNullOrEmpty(existingSummary))
        {
            summaryPrompt = $@"Summarize the following conversation in 2-3 sentences: {string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"))}";
        }
        else
        {
            summaryPrompt = $@"Update the following summary based on the new conversation messages. Keep it concise (2-3 sentences). 
                               Previous Summary: {existingSummary}.
                               New Messages : {string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"))}";
        }

        return await _kernel.InvokePromptAsync<string>(summaryPrompt);
    }

    #endregion

    #region Private Methods

    private static int GetTokenCount(object? innerContent, string tokenMetric)
    {
        if (innerContent == null)
        {
            return 0;
        }

        try
        {
            var usage = innerContent.GetType().GetProperty("Usage")
                                    ?.GetValue(innerContent);

            if (usage == null)
            {
                return 0;
            }

            var tokenValue = usage.GetType().GetProperty(tokenMetric)
                                  ?.GetValue(usage);

            return tokenValue is int count ? count : 0;
        }
        catch
        {
            return 0;
        }
    }

    private Kernel SetupKernel(Agent agent)
    {
        foreach (var toolName in agent.Tools)
        {
            var globalTools = _kernel.GetAllServices<IAgentTool>();
            var selectedTool = globalTools.FirstOrDefault(plugins => plugins.Name.Equals(toolName, StringComparison.OrdinalIgnoreCase));

            if (selectedTool == null)
            {
                _logger.LogWarning("Tool {Tool} not found for agent {AgentId}", toolName, agent.Id);

                continue;
            }

            _kernel.Plugins.AddFromObject(selectedTool, toolName);
        }

        var traceBuffer = new ToolTraceBuffer();
        _kernel.Data.Add(nameof(ToolTraceBuffer), traceBuffer);

        return _kernel;
    }

    private ChatHistory PrepareChatHistory(Agent agent, Conversation conversation, IList<Message> messages)
    {
        var chatHistory = new ChatHistory();
        var memoryConfiguration = agent.MemoryConfiguration;

        chatHistory.AddSystemMessage(agent.Instrunctions);

        if (!string.IsNullOrEmpty(conversation.Metadata.Summary))
        {
            chatHistory.AddSystemMessage($"Previous conversation summary: {conversation.Metadata.Summary}");
        }

        var recentMessages = GetRecentMessagesWithinTokenLimit(messages, memoryConfiguration.MaxContextTokens);

        foreach (var message in recentMessages)
        {
            switch (message.Role.ToLower())
            {
                case "user":
                    chatHistory.AddUserMessage(message.Content);
                    break;

                case "assistant":
                    chatHistory.AddAssistantMessage(message.Content);
                    break;

                case "system":
                    chatHistory.AddSystemMessage(message.Content);
                    break;
            }
        }

        return chatHistory;
    }

    private async Task<AgentResponseAggregate> ExecuteWithKernelAsync(Kernel kernel, ChatHistory chatHistory, Agent agent)
    {
        var modelConfiguration = agent.ModelConfiguration;

        //TODO: Adjust settings based on request provider (e.g., different settings for Azure OpenAI)
        var executionSettings = GetPromptExecutionSettings(agent);
        var chatCompletionAgent = new ChatCompletionAgent()
        {
            Kernel = kernel,
            Name = agent.Name,
            Instructions = agent.Instrunctions,
            Arguments = new KernelArguments(executionSettings)
        };
        var agentResponse = chatCompletionAgent.InvokeAsync(chatHistory);

        return await GetAgentResponseAsync(agentResponse);
    }

    private PromptExecutionSettings GetPromptExecutionSettings(Agent agent)
    {
        var config = agent.ModelConfiguration ?? throw new ArgumentNullException(nameof(agent.ModelConfiguration));

        return config.Provider switch
        {
            "OpenAI" => CreateOpenAISettings(config),
            "Azure" => CreateAzureSettings(config),
            _ => CreateDefaultSettings(config)
        };
    }

    private OpenAIPromptExecutionSettings CreateOpenAISettings(ModelConfiguration config)
    {
        var settings = new OpenAIPromptExecutionSettings();
        ApplyCommonExecutionSettings(settings, config);

        if (IsReasoningModel(config))
        {
            settings.Temperature = 1.0;
        }

        return settings;
    }

    private AzureOpenAIPromptExecutionSettings CreateAzureSettings(ModelConfiguration config)
    {
        var azureOpenAIPromptExecutionSettings = new AzureOpenAIPromptExecutionSettings();

        ApplyCommonExecutionSettings(azureOpenAIPromptExecutionSettings, config);

        if (IsReasoningModel(config))
        {
            azureOpenAIPromptExecutionSettings.SetNewMaxCompletionTokensEnabled = true;
            azureOpenAIPromptExecutionSettings.Temperature = 1.0;
        }

        return azureOpenAIPromptExecutionSettings;
    }

    private void ApplyCommonExecutionSettings(OpenAIPromptExecutionSettings settings, ModelConfiguration config)
    {
        settings.ServiceId = config.ModelName;

        settings.TopP = config.TopP;
        settings.MaxTokens = config.MaxTokens;
        settings.Temperature = config.Temperature;
        settings.PresencePenalty = config.PresencePenalty;
        settings.FrequencyPenalty = config.FrequencyPenalty;
        settings.StopSequences = config.StopSequences?.ToList();

        settings.FunctionChoiceBehavior = FunctionChoiceBehavior.Auto();
        settings.ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions;
    }

    private PromptExecutionSettings CreateDefaultSettings(ModelConfiguration config)
    {
        return new PromptExecutionSettings
        {
            ModelId = config.ModelName,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
    }

    private static bool IsReasoningModel(ModelConfiguration config) =>
        config.ModelType?.Contains("Reasoning", StringComparison.OrdinalIgnoreCase) ?? false;

    private async Task<AgentResponseAggregate> GetAgentResponseAsync(IAsyncEnumerable<AgentResponseItem<ChatMessageContent>> agentResponseItems)
    {
        var agentResponseAggregate = new AgentResponseAggregate();

        await foreach (var agentResponseItem in agentResponseItems)
        {
            if (agentResponseItem.Message is { } message)
            {
                if (!string.IsNullOrWhiteSpace(message.Content))
                {
                    agentResponseAggregate.Messages.Add(message.Content);
                }

                agentResponseAggregate.ModelId = message.ModelId;
                agentResponseAggregate.LastInnerContent = message.InnerContent;
            }
        }

        return agentResponseAggregate;
    }

    private IList<Message> GetRecentMessagesWithinTokenLimit(IList<Message> messages, int maxTokens)
    {
        var totalTokens = messages.Sum(message => message.MetaData.TokenCount);

        while (totalTokens > maxTokens && messages.Any())
        {
            var removed = messages.First();

            messages.RemoveAt(0);
            totalTokens -= removed.MetaData.TokenCount;
        }

        return messages;
    }

    #endregion
}
