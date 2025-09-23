using AutoMapper;
using GuruPR.Application.Dtos.OAuth;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Domain.Exceptions;

namespace GuruPR.Application.Services.OAuth;

public class ProviderManagementService
{
    private readonly IMapper _mapper;
    private readonly IProviderRepository _providerRepository;

    public ProviderManagementService(IProviderRepository providerRepository, IMapper mapper)
    {
        _mapper = mapper;
        _providerRepository = providerRepository;
    }

    public async Task<ProviderDto> CreateProviderAsync(CreateProviderRequest createProviderRequest)
    {
        var provider = _mapper.Map<Provider>(createProviderRequest);
        if (!provider.IsValid())
        {
            throw new DomainException("Invalid provider configuration, please recheck provider configuration.");
        }

        await _providerRepository.SaveAsync(provider);

        return _mapper.Map<ProviderDto>(provider);
    }
}
