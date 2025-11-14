using AutoMapper;
using AutoMapper.QueryableExtensions;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Common.Models;
using GuruPR.Application.Features.Agents.Dtos;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Application.Features.Agents.Queries.GetAgents;

public class GetAgentsHandler : IRequestHandler<GetAgentsQuery, PaginatedList<AgentDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAgentsHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedList<AgentDto>> Handle(GetAgentsQuery request, CancellationToken cancellationToken)
    {
        var agents = await _unitOfWork.Agents.AsQueryable()
                                             .ProjectTo<AgentDto>(_mapper.ConfigurationProvider)
                                             .AsNoTracking()
                                             .ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

        return agents;
    }
}
