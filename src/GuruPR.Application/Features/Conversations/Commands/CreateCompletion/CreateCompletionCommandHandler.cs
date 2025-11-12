using AutoMapper;

using GuruPR.Application.Features.Agents.Extensions;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Features.Conversations.Extensions;
using GuruPR.Application.Features.Conversations.Models.CreateCompletion;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Agents;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Entities.Message;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Conversations.Commands.CreateCompletion;

public class CreateCompletionCommandHandler : IRequestHandler<CreateCompletionCommand, MessageDto>
{
    private readonly ILogger<CreateCompletionCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIChatProvider _aiChatProvider;

    public CreateCompletionCommandHandler(ILogger<CreateCompletionCommandHandler> logger,
                                          IMapper mapper,
                                          IUnitOfWork unitOfWork,
                                          IAIChatProvider aiChatProvider)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _aiChatProvider = aiChatProvider;
    }

    public async Task<MessageDto> Handle(CreateCompletionCommand request, CancellationToken cancellationToken)
    {
        var agent = await _unitOfWork.Agents.GetByIdOrThrowAsync(request.AgentId, cancellationToken);

        ValidateAgentIsActive(agent);

        var conversation = await _unitOfWork.Conversations.GetByIdOrThrowAsync(request.ConversationId, cancellationToken);

        var messages = await _unitOfWork.Messages.GetMessagesAsync(request.ConversationId,
                                                                   agent.MemoryConfiguration.MaxContextMessages,
                                                                   cancellationToken);

        var agentExecutionResult = await ExecuteAIProviderAsync(agent, conversation, messages, request.Message, cancellationToken);

        var (userMessage, agentMessage) = CreateMessageEntities(conversation.Id, request.Message, agentExecutionResult);

        await PersistResultsAsync(conversation, userMessage, agentMessage, agent, agentExecutionResult, cancellationToken);

        return _mapper.Map<MessageDto>(agentMessage);
    }

    private void ValidateAgentIsActive(Agent agent)
    {
        if (!agent.IsActive())
        {
            _logger.LogWarning("Agent with ID {AgentId} is not active", agent.Id);

            throw new InvalidOperationException("The specified agent is not active. Please activate the agent and retry.");
        }
    }

    private async Task<AgentExecutionResult> ExecuteAIProviderAsync(Agent agent,
                                                                    Conversation conversation,
                                                                    IList<Message> messages,
                                                                    string userMessage,
                                                                    CancellationToken cancellationToken)
    {
        try
        {
            return await _aiChatProvider.ExecuteAsync(agent, conversation, messages, userMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI provider execution failed for Agent {AgentId} in Conversation {ConversationId}", agent.Id, conversation.Id);

            throw;
        }
    }

    private async Task PersistResultsAsync(Conversation conversation,
                                           Message userMessage,
                                           Message agentMessage,
                                           Agent agent,
                                           AgentExecutionResult agentExecutionResult,
                                           CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginGuruTransactionAsync(cancellationToken);

        try
        {
            await SaveMessagesAsync(userMessage, agentMessage, cancellationToken);

            UpdateConversation(conversation, agentExecutionResult);

            await HandleSummaryAsync(agent, conversation, cancellationToken);

            await _unitOfWork.CommitGuruTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackGuruTransactionAsync(cancellationToken);

            _logger.LogError(ex, "Failed to persist results for Conversation {ConversationId}, after completion.", conversation.Id);

            throw;
        }
    }

    private (Message user, Message assistant) CreateMessageEntities(string conversationId, string userMessageContent, AgentExecutionResult agentExecutionResult)
    {
        var userMessage = new Message
        {
            ConversationId = conversationId,
            Role = "User",
            Content = userMessageContent,
            CreatedAt = DateTime.UtcNow,
            MetaData = new MessageMetadata
            {
                TokenCount = agentExecutionResult.InputTokens
            }
        };
        var agentMessage = new Message
        {
            ConversationId = conversationId,
            Role = "Assistant",
            Content = agentExecutionResult.Content,
            ToolCalls = agentExecutionResult.ToolCalls,
            CreatedAt = DateTime.UtcNow,
            MetaData = new MessageMetadata
            {
                AgentName = agentExecutionResult.AgentName,
                TokenCount = agentExecutionResult.OutputTokens,
                ProcessingTime = agentExecutionResult.ProcessingTime,
                ModelUsed = agentExecutionResult.ModelId
            }
        };

        return (userMessage, agentMessage);
    }

    private async Task SaveMessagesAsync(Message userMessage, Message agentMessage, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Messages.AddAsync(userMessage);
        await _unitOfWork.Messages.AddAsync(agentMessage);
    }

    private void UpdateConversation(Conversation conversation, AgentExecutionResult agentExecutionResult)
    {
        conversation.UpdatedAt = DateTime.UtcNow;
        conversation.Metadata.TotalMessages += 2;
        conversation.Metadata.TotalTokens += agentExecutionResult.TotalTokens;

        _unitOfWork.Conversations.Update(conversation);
    }

    private async Task HandleSummaryAsync(Agent agent, Conversation conversation, CancellationToken cancellationToken = default)
    {
        if (agent.MemoryConfiguration.EnableSummary &&
            conversation.Metadata.TotalMessages >= agent.MemoryConfiguration.SummaryThresholdMessages)
        {
            var messages = await _unitOfWork.Messages.GetMessagesAsync(conversation.Id, agent.MemoryConfiguration.MaxContextMessages, cancellationToken);

            var updatedSummary = await _aiChatProvider.GenerateSummaryAsync(messages, conversation.Metadata.Summary, cancellationToken);

            if (string.IsNullOrEmpty(updatedSummary))
            {
                _logger.LogWarning("Summary generation returned empty for conversation {ConversationId}", conversation.Id);

                return;
            }

            conversation.Metadata.Summary = updatedSummary ?? conversation.Metadata.Summary;

            _unitOfWork.Conversations.Update(conversation);
        }
    }
}
