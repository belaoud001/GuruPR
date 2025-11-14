using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Agents.Extensions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Agents.Commands.DeleteAgent;

public class DeleteAgentHandler : IRequestHandler<DeleteAgentCommand>
{
    private readonly ILogger<DeleteAgentHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAgentHandler(ILogger<DeleteAgentHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = await _unitOfWork.Agents.GetByIdOrThrowAsync(request.Id);

        _unitOfWork.Agents.Delete(agent);

        var result = await _unitOfWork.SaveGuruChangesAsync();

        if (result == 0)
        {
            _logger.LogError("Agent {AgentId} was found but SaveChanges affected 0 rows", request.Id);

            throw new InvalidOperationException($"Failed to delete agent {request.Id}");
        }
    }
}
