using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.ProviderConnections.Dtos;
using GuruPR.Application.Features.ProviderConnections.Exceptions;
using GuruPR.Domain.Entities.ProviderConnection;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Commands.CreateProviderConnection;

public class CreateProviderConnectionHandler : IRequestHandler<CreateProviderConnectionCommand, ProviderConnectionDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProviderConnectionCommand> _validator;

    public CreateProviderConnectionHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateProviderConnectionCommand> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProviderConnectionDto> Handle(CreateProviderConnectionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderConnectionValidationException(message, errors),
                                             cancellationToken);

        var providerConnection = _mapper.Map<ProviderConnection>(request);

        var createdProviderConnection = await _unitOfWork.ProviderConnections.AddAsync(providerConnection, cancellationToken);

        await _unitOfWork.SaveGuruChangesAsync(cancellationToken);

        return _mapper.Map<ProviderConnectionDto>(createdProviderConnection);
    }
}
