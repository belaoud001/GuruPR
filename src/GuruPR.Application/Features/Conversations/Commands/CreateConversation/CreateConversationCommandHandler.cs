using AutoMapper;

using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Conversation;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Commands.CreateConversation;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = _mapper.Map<Conversation>(request);
        var createdConversation = await _unitOfWork.Conversations.AddAsync(conversation);

        await _unitOfWork.SaveGuruChangesAsync();

        return _mapper.Map<ConversationDto>(createdConversation);
    }
}
