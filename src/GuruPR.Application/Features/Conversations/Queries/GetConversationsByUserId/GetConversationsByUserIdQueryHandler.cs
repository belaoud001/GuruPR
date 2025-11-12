using AutoMapper;
using AutoMapper.QueryableExtensions;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Models;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Interfaces.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Application.Features.Conversations.Queries.GetConversationsByUserId;

public class GetConversationsByUserIdQueryHandler : IRequestHandler<GetConversationsByUserIdQuery, PaginatedList<ConversationDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetConversationsByUserIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<PaginatedList<ConversationDto>> Handle(GetConversationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var conversations = _unitOfWork.Conversations.AsQueryable()
                                                     .ProjectTo<ConversationDto>(_mapper.ConfigurationProvider)
                                                     .AsNoTracking()
                                                     .Where(conversation => conversation.UserId == request.UserId)
                                                     .OrderByDescending(c => c.UpdatedAt)
                                                     .ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

        return conversations;
    }
}
