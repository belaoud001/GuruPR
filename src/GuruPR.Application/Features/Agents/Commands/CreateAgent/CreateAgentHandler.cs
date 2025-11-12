using AutoMapper;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Exceptions.Agent;
using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Agents;

using MediatR;

namespace GuruPR.Application.Features.Agents.Commands.CreateAgent;

public class CreateAgentHandler : IRequestHandler<CreateAgentCommand, AgentDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateAgentValidator _validator = new CreateAgentValidator();

    public CreateAgentHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<AgentDto> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AgentValidationException(message, errors));

        var agent = _mapper.Map<Agent>(request);
        var createdAgent = await _unitOfWork.Agents.AddAsync(agent);

        await _unitOfWork.SaveGuruChangesAsync();

        return _mapper.Map<AgentDto>(agent);
    }
}
