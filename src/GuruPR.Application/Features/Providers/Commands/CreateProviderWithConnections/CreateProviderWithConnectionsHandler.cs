using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Providers.Dtos;
using GuruPR.Application.Features.Providers.Exceptions;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Domain.Entities.Provider;

using MediatR;

namespace GuruPR.Application.Features.Providers.Commands.CreateProviderWithConnections;

public class CreateProviderWithConnectionsHandler : IRequestHandler<CreateProviderWithConnectionsCommand, ProviderDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProviderWithConnectionsCommand> _validator;

    public CreateProviderWithConnectionsHandler(IMapper mapper, 
                                                IMediator mediator, 
                                                IUnitOfWork unitOfWork, 
                                                IValidator<CreateProviderWithConnectionsCommand> validator)
    {
        _mapper = mapper;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProviderDto> Handle(CreateProviderWithConnectionsCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderValidationException(message, errors),
                                             cancellationToken);

        var provider = _mapper.Map<Provider>(request);
        var createdProvider = await _unitOfWork.Providers.AddAsync(provider, cancellationToken);

        await _unitOfWork.SaveGuruChangesAsync(cancellationToken);

        return _mapper.Map<ProviderDto>(createdProvider);
    }
}
