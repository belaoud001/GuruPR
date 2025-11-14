using AutoMapper;

using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Application.Features.Agents.Extensions;

using MediatR;

namespace GuruPR.Application.Features.Agents.Queries.GetAgentById;

public class GetAgentByIdHandler : IRequestHandler<GetAgentByIdQuery, AgentDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAgentByIdHandler(IMapper mapper, IUnitOfWork unitOfWork)
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
