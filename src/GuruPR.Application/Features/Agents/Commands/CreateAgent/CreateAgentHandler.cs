using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Application.Features.Agents.Exceptions;
using GuruPR.Domain.Entities.Agents;

using MediatR;

namespace GuruPR.Application.Features.Agents.Commands.CreateAgent;

public class CreateAgentHandler : IRequestHandler<CreateAgentCommand, AgentDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAgentCommand> _validator;

    public CreateAgentHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateAgentCommand> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<AgentDto> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AgentValidationException(message, errors));

        var agent = _mapper.Map<Agent>(request);
        var createdAgent = await _unitOfWork.Agents.AddAsync(agent);

        await _unitOfWork.SaveGuruChangesAsync();

        return _mapper.Map<AgentDto>(agent);
    }
}
