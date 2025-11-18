using System.Linq.Expressions;

using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.ProviderConnections.Dtos;
using GuruPR.Application.Features.ProviderConnections.Exceptions;
using GuruPR.Domain.Entities.ProviderConnection;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnections;

public class GetProviderConnectionHandler : IRequestHandler<GetProviderConnectionsQuery, List<ProviderConnectionDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetProviderConnectionsQuery> _validator;

    public GetProviderConnectionHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<GetProviderConnectionsQuery> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<List<ProviderConnectionDto>> Handle(GetProviderConnectionsQuery request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderConnectionValidationException(message, errors),
                                             cancellationToken);

        Expression<Func<ProviderConnection, bool>> predicate = providerConnection => providerConnection.ProviderId == request.ProviderId;
        var providerConnections = await _unitOfWork.ProviderConnections.GetAllAsync(predicate, cancellationToken: cancellationToken);

        return _mapper.Map<List<ProviderConnectionDto>>(providerConnections);
    }
}
