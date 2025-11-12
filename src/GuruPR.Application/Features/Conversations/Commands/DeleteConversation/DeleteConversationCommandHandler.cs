using GuruPR.Application.Features.Conversations.Extensions;
using GuruPR.Application.Interfaces.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Conversations.Commands.DeleteConversation;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand>
{
    private readonly ILogger<DeleteConversationCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteConversationCommandHandler(ILogger<DeleteConversationCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginGuruTransactionAsync();

        try
        {
            await _unitOfWork.Messages.DeleteConversationMessagesAsync(request.Id, cancellationToken);

            var conversation = await _unitOfWork.Conversations.GetByIdOrThrowAsync(request.Id, cancellationToken);

            _unitOfWork.Conversations.Delete(conversation);

            var result = await _unitOfWork.SaveGuruChangesAsync();
            if (result == 0)
            {
                _logger.LogError("Conversation {ConversationId} was found but SaveChanges affected 0 rows", request.Id);

                throw new InvalidOperationException($"Failed to delete conversation {request.Id}");
            }

            await _unitOfWork.CommitGuruTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackGuruTransactionAsync();

            throw;
        }

    }
}
