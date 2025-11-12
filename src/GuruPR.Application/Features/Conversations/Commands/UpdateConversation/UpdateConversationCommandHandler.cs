using AutoMapper;

using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Features.Conversations.Extensions;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Conversation.Operations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Conversations.Commands.UpdateConversation;

public class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, ConversationDto>
{
    private readonly ILogger<UpdateConversationCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateConversationCommandHandler(ILogger<UpdateConversationCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConversationDto> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdOrThrowAsync(request.Id!, cancellationToken);
        var conversationUpdateData = _mapper.Map<ConversationUpdateData>(request);

        conversation.Update(conversationUpdateData);

        _unitOfWork.Conversations.Update(conversation);

        var result = await _unitOfWork.SaveGuruChangesAsync(cancellationToken);
        if (result == 0)
        {
            _logger.LogError("Conversation {ConversationId} was found but SaveChanges affected 0 rows", request.Id);

            throw new InvalidOperationException($"Failed to update conversation {request.Id}");
        }

        return _mapper.Map<ConversationDto>(conversation);
    }
}
