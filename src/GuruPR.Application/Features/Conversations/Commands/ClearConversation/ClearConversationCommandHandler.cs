using GuruPR.Application.Interfaces.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Conversations.Commands.ClearConversation;

public class ClearConversationCommandHandler : IRequestHandler<ClearConversationCommand>
{
    private readonly ILogger<ClearConversationCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ClearConversationCommandHandler(ILogger<ClearConversationCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ClearConversationCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Messages.DeleteConversationMessagesAsync(request.Id, cancellationToken);

        var result = await _unitOfWork.SaveGuruChangesAsync(cancellationToken);
        if (result == 0)
        {
            _logger.LogWarning("No messages were deleted for conversation {ConversationId}", request.Id);
        }
    }
}
