using AutoMapper;

using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Providers.Dtos;
using GuruPR.Application.Features.Providers.Exceptions;
using GuruPR.Application.Features.Providers.Extensions;
using GuruPR.Domain.Entities.Provider.Operations;

using MediatR;

namespace GuruPR.Application.Features.Providers.Commands.UpdateProvider;

public class UpdateProviderHandler : IRequestHandler<UpdateProviderCommand, ProviderDto>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProviderCommand> _validator;
    
    public UpdateProviderHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<UpdateProviderCommand> validator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator  = validator;
    }

    public async Task<ProviderDto> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ProviderValidationException(message, errors),
                                             cancellationToken);

        var provider = await _unitOfWork.Providers.GetByIdOrThrowAsync(request.Id, cancellationToken);
        var providerUpdateData = _mapper.Map<ProviderUpdateData>(request);

        provider.Update(providerUpdateData);

        await _unitOfWork.SaveGuruChangesAsync(cancellationToken);
        
        return _mapper.Map<ProviderDto>(provider);
    }
}
