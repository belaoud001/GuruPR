using AutoMapper;

using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Conversations.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetMessages;

public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesHandler(IUnitOfWork unitOfWork, IMapper mapper)
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
