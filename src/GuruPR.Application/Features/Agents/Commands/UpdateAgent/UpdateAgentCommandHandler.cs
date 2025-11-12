using AutoMapper;

using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Application.Features.Agents.Extensions;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Agents.Operations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Agents.Commands.UpdateAgent;

public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand, AgentDto>
{
    private readonly ILogger<UpdateAgentCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAgentCommandHandler(ILogger<UpdateAgentCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<AgentDto> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = await _unitOfWork.Agents.GetByIdOrThrowAsync(request.Id!, cancellationToken);
        var agentUpdateData = _mapper.Map<AgentUpdateData>(request);

        agent.Update(agentUpdateData);

        _unitOfWork.Agents.Update(agent);

        var result = await _unitOfWork.SaveGuruChangesAsync();

        if (result == 0)
        {
            _logger.LogError("Agent {AgentId} was found but SaveChanges affected 0 rows", request.Id);

            throw new InvalidOperationException($"Failed to update agent {request.Id}");
        }

        return _mapper.Map<AgentDto>(agent);
    }
}
