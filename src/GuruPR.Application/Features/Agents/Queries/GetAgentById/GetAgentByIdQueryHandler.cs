using AutoMapper;

using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Application.Features.Agents.Extensions;
using GuruPR.Application.Interfaces.Persistence;

using MediatR;

namespace GuruPR.Application.Features.Agents.Queries.GetAgentById;

public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, AgentDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAgentByIdQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<AgentDto> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
    {
        var agent = await _unitOfWork.Agents.GetByIdOrThrowAsync(request.Id);

        return _mapper.Map<AgentDto>(agent);
    }
}
