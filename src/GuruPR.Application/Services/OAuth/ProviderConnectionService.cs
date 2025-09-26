using AutoMapper;

using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Application.Dtos.OAuth.ProviderConnection;

namespace GuruPR.Application.Services.OAuth;

public class ProviderConnectionService : IProviderConnectionService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    #region Public Methods

    public ProviderConnectionService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProviderConnectionDto> AddProviderConnectionToProviderAsync(string providerId, CreateProviderConnectionRequest createProviderConnectionRequest)
    {
        var provider = await GetProviderByIdOrThrowExceptionAsync(providerId);
        var providerConnection = _mapper.Map<ProviderConnection>(createProviderConnectionRequest);

        provider.AddProviderConnection(providerConnection);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProviderConnectionDto>(providerConnection);
    }

    public async Task<bool> DeleteProviderConnectionAsync(string providerId, string providerConnectionId)
    {
        var provider = await GetProviderByIdOrThrowExceptionAsync(providerId);

        var removed = provider.RemoveProviderConnection(providerConnectionId);

        if (!removed)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<ProviderConnectionDto>> GetConnectionsByProviderAsync(string providerId)
    {
        var provider = await GetProviderByIdOrThrowExceptionAsync(providerId);

        return _mapper.Map<IEnumerable<ProviderConnectionDto>>(provider.ProviderConnections);
    }

    #endregion

    #region Private Methods

    private async Task<Provider> GetProviderByIdOrThrowExceptionAsync(string providerId)
    {
        var provider = await _unitOfWork.Providers.GetByIdAsync(providerId);
        if (provider is null)
        {
            throw new NotFoundException($"Provider with ID {providerId} not found.");
        }

        return provider;
    }

    #endregion
}
