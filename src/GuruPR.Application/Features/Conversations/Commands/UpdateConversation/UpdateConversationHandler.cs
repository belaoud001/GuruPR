using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Features.Conversations.Exceptions;
using GuruPR.Application.Features.Conversations.Extensions;
using GuruPR.Domain.Entities.Conversation.Operations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Conversations.Commands.UpdateConversation;

public class UpdateConversationHandler : IRequestHandler<UpdateConversationCommand, ConversationDto>
{
    private readonly ILogger<UpdateConversationHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateConversationCommand> _validator;

    public UpdateConversationHandler(ILogger<UpdateConversationHandler> logger,
                                            IMapper mapper,
                                            IUnitOfWork unitOfWork,
                                            IValidator<UpdateConversationCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ConversationDto> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ConversationValidationException(message, errors));

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
