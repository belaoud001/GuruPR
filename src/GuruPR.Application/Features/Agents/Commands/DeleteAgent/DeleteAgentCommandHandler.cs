using GuruPR.Application.Features.Agents.Extensions;
using GuruPR.Application.Interfaces.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Agents.Commands.DeleteAgent;

public class DeleteAgentCommandHandler : IRequestHandler<DeleteAgentCommand>
{
    private readonly ILogger<DeleteAgentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAgentCommandHandler(ILogger<DeleteAgentCommandHandler> logger, IUnitOfWork unitOfWork)
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
