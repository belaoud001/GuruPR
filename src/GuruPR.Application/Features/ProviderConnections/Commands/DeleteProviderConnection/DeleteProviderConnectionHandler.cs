using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.ProviderConnections.Exceptions;
using GuruPR.Application.Features.ProviderConnections.Extensions;
using GuruPR.Application.Features.Providers.Extensions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.ProviderConnections.Commands.DeleteProviderConnection;

public class DeleteProviderConnectionHandler : IRequestHandler<DeleteProviderConnectionCommand>
{
    private readonly ILogger<DeleteProviderConnectionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProviderConnectionCommand> _validator;

    public DeleteProviderConnectionHandler(ILogger<DeleteProviderConnectionHandler> logger,
                                           IUnitOfWork unitOfWork,
                                           IValidator<DeleteProviderConnectionCommand> validator)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task Handle(DeleteProviderConnectionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderConnectionValidationException(message, errors),
                                             cancellationToken);

        var provider = await _unitOfWork.ProviderConnections.GetByIdOrThrowAsync(request.Id);

        _unitOfWork.ProviderConnections.Delete(provider);

        var result = await _unitOfWork.SaveGuruChangesAsync();

        if (result == 0)
        {
            _logger.LogError("Provider Connection {providerConnectionId} was found but SaveChanges affected 0 rows", request.Id);

            throw new InvalidOperationException($"Failed to delete provider connection {request.Id}");
        }
    }
}
