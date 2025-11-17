using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.ProviderConnections.Dtos;
using GuruPR.Application.Features.ProviderConnections.Exceptions;
using GuruPR.Application.Features.ProviderConnections.Extensions;
using GuruPR.Application.Features.Providers.Extensions;
using GuruPR.Domain.Entities.ProviderConnection.Operations;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Commands.UpdateProviderConnection;

public class UpdateProviderConnectionHandler : IRequestHandler<UpdateProviderConnectionCommand, ProviderConnectionDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProviderConnectionCommand> _validator;

    public UpdateProviderConnectionHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<UpdateProviderConnectionCommand> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProviderConnectionDto> Handle(UpdateProviderConnectionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request, 
                                             (message, errors) => new ProviderConnectionValidationException(message, errors),
                                             cancellationToken);

        var providerConnection = await _unitOfWork.ProviderConnections.GetByIdOrThrowAsync(request.ProviderConnectionId!, cancellationToken);
        var providerConnectionUpdateData = _mapper.Map<ProviderConnectionUpdateData>(request);

        providerConnection.Update(providerConnectionUpdateData);

        _unitOfWork.ProviderConnections.Update(providerConnection);

        await _unitOfWork.SaveGuruChangesAsync(cancellationToken);

        return _mapper.Map<ProviderConnectionDto>(providerConnection);
    }
}
