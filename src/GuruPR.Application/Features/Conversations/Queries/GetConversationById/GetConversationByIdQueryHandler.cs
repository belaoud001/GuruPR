using AutoMapper;

using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Interfaces.Persistence;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetConversationById;

public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, ConversationDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetConversationByIdQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ConversationDto> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(request.Id, cancellationToken);

        return _mapper.Map<ConversationDto>(conversation);
    }
}
