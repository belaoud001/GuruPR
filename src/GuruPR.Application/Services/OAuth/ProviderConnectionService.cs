using AutoMapper;

using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Enums;
using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Services.OAuth;

public class ProviderConnectionService : IProviderConnectionService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProviderConnectionService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    #region Public Methods

    public async Task<IEnumerable<ProviderConnection>> GetConnectionsByProviderIdAsync(string providerId)
    {
        var provider = await GetProviderByIdOrThrowExceptionAsync(providerId);

        return provider.ProviderConnections ?? [];
    }

    public async Task<ProviderConnection> GetProviderConnectionByIdAsync(string providerId, string providerConnectionId)
    {
        var provider = await GetProviderByIdOrThrowExceptionAsync(providerId);
        var providerConnection = provider.ProviderConnections.FirstOrDefault(providerConnection => providerConnection.Id == providerConnectionId);

        if (providerConnection is null)
        {
            throw new NotFoundException($"Provider connection with ID '{providerConnectionId}' not found for provider with ID '{providerId}'.");
        }

        return providerConnection;
    }

    public async Task<ProviderConnection> GetProviderConnectionByScopeAndProviderNameAsync(string providerName, string scope)
    {
        var provider = await _unitOfWork.Providers.GetProviderByNameAsync(providerName);

        if (provider is null)
        {
            throw new NotFoundException($"Provider with name '{providerName}' not found.");
        }

        var providerConnection = provider?.ProviderConnections.FirstOrDefault(pc => pc.HasScope(scope));

        if (providerConnection is null)
        {
            throw new NotFoundException($"Provider connection with scope '{scope}' for provider '{providerName}' not found.");
        }

        return providerConnection;
    }

    public async Task<ProviderConnection> GetProviderConnectionByProviderTypeAndScopeAsync(OAuthProviderType OAuthProviderType, string scope)
    {
        var provider = await GetProviderByTypeOrThrowExceptionAsync(OAuthProviderType);
        var providerConnection = provider?.ProviderConnections.FirstOrDefault(pc => pc.HasScope(scope));

        if (providerConnection is null)
        {
            throw new NotFoundException($"Provider connection with scope '{scope}' for provider type '{OAuthProviderType}' not found.");
        }

        return providerConnection;
    }


    public async Task<ProviderConnection> AddProviderConnectionToProviderAsync(string providerId, CreateProviderConnectionRequest createProviderConnectionRequest)
    {
        var provider = await GetProviderByIdOrThrowExceptionAsync(providerId);
        var providerConnection = _mapper.Map<ProviderConnection>(createProviderConnectionRequest);

        provider.AddProviderConnection(providerConnection);
        await _unitOfWork.SaveGuruChangesAsync();

        return providerConnection;
    }

    public async Task<ProviderConnection> UpdateProviderConnectionByProviderTypeAsync(OAuthProviderType OAuthProviderType, 
                                                                                      string providerConnectionId, 
                                                                                      UpdateProviderConnectionRequest updateProviderConnectionRequest)
    {
        var provider = await GetProviderByTypeOrThrowExceptionAsync(OAuthProviderType);
        var providerConnection = await GetProviderConnectionByIdAsync(provider.Id, providerConnectionId);

        _mapper.Map(updateProviderConnectionRequest, providerConnection);
        await _unitOfWork.SaveGuruChangesAsync();

        return providerConnection;
    }

    public async Task<bool> DeleteProviderConnectionAsync(string providerId, string providerConnectionId)
    {
        var provider = await GetProviderByIdOrThrowExceptionAsync(providerId);

        var removed = provider.RemoveProviderConnection(providerConnectionId);

        if (!removed)
        {
            return false;
        }

        await _unitOfWork.SaveGuruChangesAsync();
        
        return true;
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

    private async Task<Provider> GetProviderByTypeOrThrowExceptionAsync(OAuthProviderType OAuthProviderType)
    {
        var provider = await _unitOfWork.Providers.GetProviderByTypeAsync(OAuthProviderType);

        if (provider is null)
        {
            throw new NotFoundException($"Provider with type '{OAuthProviderType}' not found.");
        }

        return provider;
    }

    #endregion
}
