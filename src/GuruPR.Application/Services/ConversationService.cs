using AutoMapper;

using GuruPR.Application.Dtos.Conversation;
using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Interfaces.Infrastructure.SemanticKernel.Models;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Entities.Configurations.Enums;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Entities.Message;
using GuruPR.Domain.Requests;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Services;

public class ConversationService : IConversationService
{
    private readonly ILogger<ConversationService> _logger;
    private readonly IAIChatProvider _aiChatProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ConversationService(ILogger<ConversationService> logger,
                               IAIChatProvider aiChatProvider,
                               IUnitOfWork unitOfWork,
                               IMapper mapper)
    {
        _logger = logger;
        _aiChatProvider = aiChatProvider;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    #region Public Methods

    public async Task<List<Conversation>> GetAllConversationsByUserIdAsync(string userId)
    {
        var conversations = await _unitOfWork.Conversations.GetAllByUserIdAsync(userId);

        return conversations;
    }

    public async Task<Conversation> GetConversationByIdAsync(string conversationId, string userId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);

        if (conversation == null)
        {
            throw new NotFoundException("Conversation with the given ID {conversationId} not found");
        }

        if (conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not have access to this conversation.");
        }

        return conversation;
    }

    public async Task<Conversation> CreateConversationAsync(CreateConversationRequest createConversationRequest, string userId)
    {
        var conversation = _mapper.Map<Conversation>(createConversationRequest);

        conversation.UserId = userId;

        var createdConversation = await _unitOfWork.Conversations.AddAsync(conversation);

        await _unitOfWork.SaveGuruChangesAsync();

        return createdConversation;
    }

    public async Task<Conversation> UpdateConversationAsync(string conversationId, string userId, UpdateConversationRequest updateConversationRequest)
    {
        var conversation = await GetConversationByIdAsync(conversationId, userId);
        var updatedConversation = _mapper.Map(updateConversationRequest, conversation);

        _unitOfWork.Conversations.Update(conversation);

        await _unitOfWork.SaveGuruChangesAsync();

        return conversation;
    }

    public async Task<bool> DeleteConversationAsync(string conversationId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
        if (conversation == null)
        {
            throw new NotFoundException("Conversation with the given ID {conversationId} not found");
        }

        await _unitOfWork.Messages.DeleteConversationMessagesAsync(conversationId);
        _unitOfWork.Conversations.Delete(conversation);

        var result = await _unitOfWork.SaveGuruChangesAsync();
        return result > 0;
    }

    public async Task RunAgentWorkflowAsync(AgentExecutionRequest request, string userId)
    {
        var startTime = DateTime.UtcNow;

        var agent = await GetActiveAgentAsync(request.AgentId);
        var conversation = await ValidateConversationAsync(request.ConversationId, userId);
        var messages = await _unitOfWork.Messages.GetMessagesAsync(conversation.Id,
                                                                   agent.MemoryConfiguration.MaxContextMessages);

        var result = await _aiChatProvider.ExecuteAsync(agent, conversation, messages, request.Message);

        await SaveMessagesAsync(conversation, request.Message, result);

        await HandleSummaryAsync(agent, conversation);
    }

    #endregion

    #region Private Methods

    private async Task<Agent> GetActiveAgentAsync(string agentId)
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

        // Add admin check 

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

        await _unitOfWork.SaveGuruChangesAsync();
    }

    private async Task HandleSummaryAsync(Agent agent, Conversation conversation)
    {
        if (agent.MemoryConfiguration.EnableSummary &&
            conversation.Metadata.TotalMessages >= agent.MemoryConfiguration.SummaryThresholdMessages)
        {
            var messages = await _unitOfWork.Messages.GetMessagesAsync(conversation.Id, agent.MemoryConfiguration.MaxContextMessages);

            var updatedSummary = await _aiChatProvider.GenerateSummaryAsync(messages, conversation.Metadata.Summary);

            if (string.IsNullOrEmpty(updatedSummary))
            {
                _logger.LogWarning("Summary generation returned empty for conversation {ConversationId}", conversation.Id);

                return;
            }

            conversation.Metadata.Summary = updatedSummary ?? conversation.Metadata.Summary;

            _unitOfWork.Conversations.Update(conversation);

            await _unitOfWork.SaveGuruChangesAsync();
        }
    }

    #endregion
}
