using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Application.Features.Agents.Exceptions;
using GuruPR.Application.Features.Agents.Extensions;
using GuruPR.Domain.Entities.Agents.Operations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Agents.Commands.UpdateAgent;

public class UpdateAgentHandler : IRequestHandler<UpdateAgentCommand, AgentDto>
{
    private readonly ILogger<UpdateAgentHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateAgentCommand> _validator;

    public UpdateAgentHandler(ILogger<UpdateAgentHandler> logger,
                              IMapper mapper,
                              IUnitOfWork unitOfWork,
                              IValidator<UpdateAgentCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<AgentDto> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AgentValidationException(message, errors));

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
