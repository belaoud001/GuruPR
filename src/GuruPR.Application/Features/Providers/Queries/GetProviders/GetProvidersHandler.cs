using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Providers.Dtos;
using GuruPR.Application.Features.Providers.Exceptions;

using MediatR;

namespace GuruPR.Application.Features.Providers.Queries.GetProviders;

public class GetProvidersHandler : IRequestHandler<GetProvidersQuery, List<ProviderDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GetProvidersQuery> _validator;

    public GetProvidersHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<GetProvidersQuery> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<List<ProviderDto>> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderValidationException(message, errors),
                                             cancellationToken);

        var providers = await _unitOfWork.Providers.GetAllAsync(cancellationToken: cancellationToken);
        return _mapper.Map<List<ProviderDto>>(providers);
    }
}
