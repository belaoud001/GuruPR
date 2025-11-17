using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Providers.Exceptions;
using GuruPR.Application.Features.Providers.Extensions;

using MediatR;

namespace GuruPR.Application.Features.Providers.Commands.DeleteProvider;

public class DeleteProviderHandler : IRequestHandler<DeleteProviderCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProviderCommand> _validator;

    public DeleteProviderHandler(IUnitOfWork unitOfWork, IValidator<DeleteProviderCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _validator  = validator;
    }

    public async Task Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderValidationException(message, errors),
                                             cancellationToken);

        var provider = await _unitOfWork.Providers.GetByIdOrThrowAsync(request.Id, cancellationToken);

        _unitOfWork.Providers.Delete(provider);

        await _unitOfWork.SaveGuruChangesAsync(cancellationToken);
    }
}
