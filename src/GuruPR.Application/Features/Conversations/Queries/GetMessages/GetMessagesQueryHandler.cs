using AutoMapper;

using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Interfaces.Persistence;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _unitOfWork.Messages.GetMessagesAsync(request.ConversationId);

        return _mapper.Map<List<MessageDto>>(messages);
    }
}
