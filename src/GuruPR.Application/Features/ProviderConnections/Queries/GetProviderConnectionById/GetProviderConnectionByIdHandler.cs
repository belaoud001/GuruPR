using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.ProviderConnections.Dtos;
using GuruPR.Application.Features.ProviderConnections.Exceptions;
using GuruPR.Application.Features.ProviderConnections.Extensions;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnectionById;

public class GetProviderConnectionByIdHandler : IRequestHandler<GetProviderConnectionByIdQuery, ProviderConnectionDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetProviderConnectionByIdQuery> _validator;

    public GetProviderConnectionByIdHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<GetProviderConnectionByIdQuery> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProviderConnectionDto> Handle(GetProviderConnectionByIdQuery request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderConnectionValidationException(message, errors),
                                             cancellationToken);

        var providerConnection = await _unitOfWork.ProviderConnections.GetByIdOrThrowAsync(request.Id, cancellationToken);

        return _mapper.Map<ProviderConnectionDto>(providerConnection);
    }
}
