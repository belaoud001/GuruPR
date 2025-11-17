using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.ProviderConnections.Exceptions;
using GuruPR.Application.Features.ProviderConnections.Extensions;
using GuruPR.Application.Features.Providers.Extensions;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Commands.DeleteProviderConnection;

public class DeleteProviderConnectionHandler : IRequestHandler<DeleteProviderConnectionCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProviderConnectionCommand> _validator;

    public DeleteProviderConnectionHandler(IUnitOfWork unitOfWork, IValidator<DeleteProviderConnectionCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator  = validator;
    } 

    public async Task Handle(DeleteProviderConnectionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderConnectionValidationException(message, errors),
                                             cancellationToken);

        var provider = await _unitOfWork.ProviderConnections.GetByIdOrThrowAsync(request.ProviderConnectionId);

        _unitOfWork.ProviderConnections.Delete(provider);

        await _unitOfWork.SaveGuruChangesAsync(cancellationToken);
    }
}
