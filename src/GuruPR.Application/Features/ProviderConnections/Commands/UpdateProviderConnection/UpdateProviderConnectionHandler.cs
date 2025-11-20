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

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.ProviderConnections.Commands.UpdateProviderConnection;

public class UpdateProviderConnectionHandler : IRequestHandler<UpdateProviderConnectionCommand, ProviderConnectionDto>
{
    private readonly ILogger<UpdateProviderConnectionHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProviderConnectionCommand> _validator;

    public UpdateProviderConnectionHandler(ILogger<UpdateProviderConnectionHandler> logger,
                                           IMapper mapper,
                                           IUnitOfWork unitOfWork,
                                           IValidator<UpdateProviderConnectionCommand> validator)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProviderConnectionDto> Handle(UpdateProviderConnectionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderConnectionValidationException(message, errors),
                                             cancellationToken);

        var providerConnection = await _unitOfWork.ProviderConnections.GetByIdOrThrowAsync(request.Id!, cancellationToken);
        var providerConnectionUpdateData = _mapper.Map<ProviderConnectionUpdateData>(request);

        providerConnection.Update(providerConnectionUpdateData);

        _unitOfWork.ProviderConnections.Update(providerConnection);

        var result = await _unitOfWork.SaveGuruChangesAsync(cancellationToken);

        if (result == 0)
        {
            _logger.LogError("Provider Connection {providerConnectionId} was found but SaveChanges affected 0 rows", request.Id);

            throw new InvalidOperationException($"Failed to delete provider connection {request.Id}");
        }

        return _mapper.Map<ProviderConnectionDto>(providerConnection);
    }
}
