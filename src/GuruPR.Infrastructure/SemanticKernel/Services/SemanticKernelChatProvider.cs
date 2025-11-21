using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Common.Interfaces.Infrastructure.SemanticKernel.Plugins;
using GuruPR.Application.Features.Conversations.Models.CreateCompletion;
using GuruPR.Application.Features.Conversations.Models.ToolCalls;
using GuruPR.Domain.Entities.Agents.Configurations;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Entities.Message;
using GuruPR.Domain.Entities.Tool;
using GuruPR.Infrastructure.SemanticKernel.Models;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Agent = GuruPR.Domain.Entities.Agents.Agent;

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

    public async Task<AgentExecutionResult> ExecuteAsync(Agent agent, Conversation conversation, IList<Message> messages, string userMessage, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;

        var kernel = SetupKernel(agent);
        var chatHistory = PrepareChatHistory(agent, conversation, messages, userMessage);
        var response = await ExecuteWithKernelAsync(kernel, chatHistory, agent);

        kernel.Data.TryGetValue(nameof(ToolTraceBuffer), out var trace);

        ToolTraceBuffer? toolTraceBuffer = kernel.Data.TryGetValue(nameof(ToolTraceBuffer), out var toolTrace)
                                                       ? toolTrace as ToolTraceBuffer : null;

        var toolCalls = toolTraceBuffer?.Events.Select(@event => new ToolCall
        {
            Name = @event.FunctionName,
            PluginName = @event.PluginName,
            Arguments = @event.ArgumentsJson,
            Output = @event.OutputJson ?? string.Empty
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

            InputTokens = GetTokenCount(response.LastInnerContent, Constants.InputTokenCount),
            OutputTokens = GetTokenCount(response.LastInnerContent, Constants.OutputTokenCount),
            TotalTokens = GetTokenCount(response.LastInnerContent, Constants.TotalTokenCount),

            ToolCalls = toolCalls,

            ProcessingTime = DateTime.UtcNow - startTime
        };
    }

    public async Task<string?> GenerateSummaryAsync(IList<Message> messages, string? existingSummary, CancellationToken cancellationToken)
    {
        string summaryPrompt;

        if (string.IsNullOrEmpty(existingSummary))
        {
            summaryPrompt = $@"You are an expert conversation summarizer. Summarize the following conversation clearly and concisely in 4-5 sentences. 
                               Include the key points, decisions, or actions mentioned. 
                               Do not add any personal opinions or unnecessary details.

                               Conversation:
                               {string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"))}";
        }
        else
        {
            summaryPrompt = $@"You are an expert conversation summarizer. Update the existing summary to reflect new conversation messages. 
                               Keep it concise (4-5 sentences), preserving previously mentioned key points unless they have been contradicted or updated. 
                               Include any new important points, decisions, or actions, and avoid repeating trivial details.

                               Previous Summary:
                               {existingSummary}

                               New Messages:
                               {string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"))}";
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
            var usage = innerContent.GetType().GetProperty("Usage")?.GetValue(innerContent);
            if (usage == null)
            {
                return 0;
            }

            var tokenValue = usage.GetType().GetProperty(tokenMetric)?.GetValue(usage);

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

    private ChatHistory PrepareChatHistory(Agent agent, Conversation conversation, IList<Message> messages, string userMessage)
    {
        var chatHistory = new ChatHistory();
        var memoryConfiguration = agent.MemoryConfiguration;

        chatHistory.AddSystemMessage(agent.Instructions);

        if (conversation.Metadata != null && !string.IsNullOrEmpty(conversation.Metadata.Summary))
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

        chatHistory.AddUserMessage(userMessage);

        return chatHistory;
    }

    private async Task<AgentResponseAggregate> ExecuteWithKernelAsync(Kernel kernel, ChatHistory chatHistory, Agent agent)
    {
        var modelConfiguration = agent.ModelConfiguration;

        //TODO: Adjust settings based on request provider (e.g., different settings for Azure OpenAI)
        var executionSettings = GetPromptExecutionSettings(agent);
        var safeAgentName = agent.Name.Replace(" ", "_");
        var chatCompletionAgent = new ChatCompletionAgent()
        {
            Kernel = kernel,
            Name = safeAgentName,
            Instructions = agent.Instructions,
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

        if (config.IsReasoningModel())
        {
            settings.Temperature = 1.0;
        }

        return settings;
    }

    private AzureOpenAIPromptExecutionSettings CreateAzureSettings(ModelConfiguration config)
    {
        var azureOpenAIPromptExecutionSettings = new AzureOpenAIPromptExecutionSettings();

        ApplyCommonExecutionSettings(azureOpenAIPromptExecutionSettings, config);

        if (config.IsReasoningModel())
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
    }

    private PromptExecutionSettings CreateDefaultSettings(ModelConfiguration config)
    {
        return new PromptExecutionSettings
        {
            ModelId = config.ModelName,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
    }

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
