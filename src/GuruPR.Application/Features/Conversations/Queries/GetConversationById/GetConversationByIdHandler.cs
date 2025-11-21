using AutoMapper;

using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Conversations.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetConversationById;

public class GetConversationByIdHandler : IRequestHandler<GetConversationByIdQuery, ConversationDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetConversationByIdHandler(IMapper mapper, IUnitOfWork unitOfWork)
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
