using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Features.Conversations.Exceptions;
using GuruPR.Domain.Entities.Conversation;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Commands.CreateConversation;

public class CreateConversationHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateConversationCommand> _validator;

    public CreateConversationHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateConversationCommand> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ConversationValidationException(message, errors));

        var conversation = _mapper.Map<Conversation>(request);
        var createdConversation = await _unitOfWork.Conversations.AddAsync(conversation);

        await _unitOfWork.SaveGuruChangesAsync();

        return _mapper.Map<ConversationDto>(createdConversation);
    }
}
