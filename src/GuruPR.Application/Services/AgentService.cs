using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Entities.Configurations.Enums;
using GuruPR.Domain.Requests;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Services;

public class AgentService : IAgentService
{
    private readonly ILogger<AgentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIChatProvider _aiChatProvider;

    public AgentService(ILogger<AgentService> logger,
                        IUnitOfWork unitOfWork,
                        IAIChatProvider aiChatProvider)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _aiChatProvider = aiChatProvider;
    }

    #region Public Methods

    public async Task<IEnumerable<Agent>> GetAllAgentsAsync(string? userId = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return await _unitOfWork.Agents.GetAllAsync();
        }

        return await _unitOfWork.Agents.GetAllAgentsAsync(userId);
    }

    public async Task<string> ExecuteAgentAsync(AgentExecutionRequest request, string userId)
    {
        var startTime = DateTime.UtcNow;

        var agent = await ValidateAgentAsync(request.AgentId);
        var conversation = await ValidateConversationAsync(request.ConversationId, userId);
        var messages = await _unitOfWork.Messages.GetMessagesAsync(conversation.Id,
                                                                   agent.MemoryConfiguration.MaxContextMessages);

        var result = await _aiChatProvider.ExecuteAsync(agent, conversation, messages, request.Message);

        await SaveMessagesAsync(conversation, request.Message, result);

        await HandleSummaryAsync(agent, conversation);

        await _unitOfWork.SaveGuruChangesAsync();

        return result.Content;
    }

    private async Task<Agent> ValidateAgentAsync(string agentId)
    {
        var agent = await _unitOfWork.Agents.GetByIdAsync(agentId);
        if (agent == null)
        {
            throw new NotFoundException($"Agent with ID {agentId} not found.");
        }

        if (agent.Status != AgentStatus.Active)
        {
            throw new InvalidOperationException($"Agent with ID {agentId} is not active.");
        }

        return agent;
    }

    #endregion

    #region Private Methods

    private async Task<Conversation> ValidateConversationAsync(string conversationId, string userId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);

        if (conversation == null)
        {
            throw new NotFoundException($"Conversation with ID {conversationId} not found.");
        }

        if (conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not have access to this conversation.");
        }

        return conversation;
    }

    private async Task SaveMessagesAsync(Conversation conversation, string userMessageContent, AgentExecutionResult result)
    {
        var userMessage = new Message
        {
            ConversationId = conversation.Id,
            Role = "User",
            Content = userMessageContent,
            CreatedAt = DateTime.UtcNow,
            MetaData = new MessageMetadata
            {
                TokenCount = result.InputTokens
            }
        };
        var agentMessage = new Message
        {
            ConversationId = conversation.Id,
            Role = "Assistant",
            Content = result.Content,
            ToolCalls = result.ToolCalls,
            CreatedAt = DateTime.UtcNow,
            MetaData = new MessageMetadata
            {
                AgentName = result.AgentName,
                TokenCount = result.OutputTokens,
                ProcessingTime = result.ProcessingTime,
                ModelUsed = result.ModelId
            }
        };

        await _unitOfWork.Messages.AddAsync(userMessage);
        await _unitOfWork.Messages.AddAsync(agentMessage);

        conversation.UpdatedAt = DateTime.UtcNow;
        conversation.Metadata.TotalMessages += 2;
        conversation.Metadata.TotalTokens += result.TotalTokens;

        _unitOfWork.Conversations.Update(conversation);
    }

    private async Task HandleSummaryAsync(Agent agent, Conversation conversation)
    {
        if (agent.MemoryConfiguration.EnableSummary &&
            conversation.Metadata.TotalMessages >= agent.MemoryConfiguration.SummaryThresholdMessages)
        {
            var messages = await _unitOfWork.Messages.GetMessagesAsync(conversation.Id, 4);

            var updatedSummary = await _aiChatProvider.GenerateSummaryAsync(messages, conversation.Metadata.Summary);

            if (string.IsNullOrEmpty(updatedSummary))
            {
                _logger.LogWarning("Summary generation returned empty for conversation {ConversationId}", conversation.Id);

                return;
            }

            conversation.Metadata.Summary = updatedSummary ?? conversation.Metadata.Summary;

            _unitOfWork.Conversations.Update(conversation);
        }

        _unitOfWork.Conversations.Update(conversation);
    }

    #endregion
}
