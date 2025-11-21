using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Providers.Exceptions;
using GuruPR.Application.Features.Providers.Extensions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Providers.Commands.DeleteProvider;

public class DeleteProviderHandler : IRequestHandler<DeleteProviderCommand>
{
    private readonly ILogger<DeleteProviderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProviderCommand> _validator;

    public DeleteProviderHandler(ILogger<DeleteProviderHandler> logger,
                                 IUnitOfWork unitOfWork,
                                 IValidator<DeleteProviderCommand> validator)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderValidationException(message, errors),
                                             cancellationToken);

        var provider = await _unitOfWork.Providers.GetByIdOrThrowAsync(request.Id, cancellationToken);

        _unitOfWork.Providers.Delete(provider);

        await _unitOfWork.ProviderConnections.DeleteProviderConnectionsByProviderIdAsync(request.Id, cancellationToken);

        var result = await _unitOfWork.SaveGuruChangesAsync(cancellationToken);

        if (result == 0)
        {
            _logger.LogError("Provider {ProviderId} was found but SaveChanges affected 0 rows", request.Id);

            throw new InvalidOperationException($"Failed to delete provider {request.Id}");
        }
    }
}
