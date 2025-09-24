using AutoMapper;

using GuruPR.Application.Dtos.OAuth;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Domain.Exceptions;

namespace GuruPR.Application.Services.OAuth;

public class ProviderManagementService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProviderManagementService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProviderDto> CreateProviderAsync(CreateProviderRequest createProviderRequest)
    {
        var provider = _mapper.Map<Provider>(createProviderRequest);
        if (!provider.IsValid())
        {
            throw new DomainException("Invalid provider configuration, please recheck provider configuration.");
        }

        await _unitOfWork.Providers.AddAsync(provider);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProviderDto>(provider);
    }

    public async Task<bool> TestDatabaseConnectionAsync()
    {
        try
        {
            return await _unitOfWork.TestAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return false;
        }
    }
}
