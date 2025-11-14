using AutoMapper;

using GuruPR.Application.Common.Exceptions;
using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Dtos.OAuth.Provider;
using GuruPR.Domain.Entities.Enums;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Domain.Exceptions;

namespace GuruPR.Application.Services.OAuth;

/// <summary>
/// Service for managing OAuth providers and their connections, 
/// including creation, retrieval, updating, and deletion.
/// </summary>
public class ProviderService : IProviderService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProviderService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    #region Public Methods

    /// <summary>
    /// Creates a new OAuth provider and saves it to the database.
    /// </summary>
    /// <param name="createProviderRequest">The provider entity to create.</param>
    /// <returns>The created provider entity.</returns>
    /// <exception cref="DomainException">Thrown when the provider configuration is invalid.</exception>
    public async Task<Provider> CreateProviderAsync(CreateProviderRequest createProviderRequest)
    {
        var provider = _mapper.Map<Provider>(createProviderRequest);
        if (!provider.IsValid())
        {
            throw new DomainException("Invalid provider configuration, please recheck provider configuration.");
        }

        await _unitOfWork.Providers.AddAsync(provider);
        await _unitOfWork.SaveGuruChangesAsync();

        return provider;
    }

    /// <summary>
    /// Retrieves all OAuth providers from the database.
    /// </summary>
    /// <returns>A collection of provider entities.</returns>
    public async Task<List<Provider>> GetAllProvidersAsync()
    {
        var providers = await _unitOfWork.Providers.GetAllAsync();

        return providers;
    }

    /// <summary>
    /// Retrieves an OAuth provider by its unique identifier.
    /// </summary>
    /// <param name="providerId">The unique identifier of the provider.</param>
    /// <returns>The provider entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<Provider> GetProviderByIdAsync(string providerId)
    {
        var provider = await GetProviderOrThrowAsync(providerId);

        return provider;
    }

    /// <summary>
    /// Retrieves an OAuth provider by its name.
    /// </summary>
    /// <param name="providerName">The name of the provider.</param>
    /// <returns>The provider entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<Provider> GetProviderByNameAsync(string providerName)
    {
        var provider = await _unitOfWork.Providers.GetProviderByNameAsync(providerName);

        if (provider == null)
        {
            throw new NotFoundException($"Provider with name {providerName} not found.");
        }

        return provider;
    }

    /// <summary>
    /// Retrieves an OAuth provider by its type.
    /// </summary>
    /// <param name="OAuthProviderType">The type of the provider.</param>
    /// <returns>The provider entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<Provider> GetProviderByTypeAsync(OAuthProviderType OAuthProviderType)
    {
        var provider = await _unitOfWork.Providers.GetProviderByTypeAsync(OAuthProviderType);

        if (provider == null)
        {
            throw new NotFoundException($"Provider with type {OAuthProviderType} not found.");
        }

        return provider;
    }

    /// <summary>
    /// Updates an existing OAuth provider.
    /// </summary>
    /// <param name="providerId">The ID of the provider to update.</param>
    /// <param name="updateProviderRequest">Request containing updated provider details.</param>
    /// <returns>The updated provider entity.</returns>
    /// <exception cref="DomainException">Thrown when the updated provider configuration is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<Provider> UpdateProviderAsync(string providerId, UpdateProviderRequest updateProviderRequest)
    {
        var provider = await GetProviderOrThrowAsync(providerId);

        _mapper.Map(updateProviderRequest, provider);

        if (!provider.IsValid())
        {
            throw new DomainException("Invalid provider configuration, please recheck provider configuration.");
        }

        _unitOfWork.Providers.Update(provider);
        await _unitOfWork.SaveGuruChangesAsync();

        return provider;
    }

    /// <summary>
    /// Deletes an OAuth provider by its unique identifier.
    /// </summary>
    /// <param name="providerId">The unique identifier of the provider to delete.</param>
    /// <returns>True if the provider was successfully deleted; otherwise, false.</returns>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<bool> DeleteProviderAsync(string providerId)
    {
        var provider = await GetProviderOrThrowAsync(providerId);

        _unitOfWork.Providers.Delete(provider);

        var result = await _unitOfWork.SaveGuruChangesAsync();
        return result > 0;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Retrieves an OAuth provider by its unique identifier or throws an exception if not found.
    /// </summary>
    /// <param name="providerId">The unique identifier of the provider.</param>
    /// <returns>The provider entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    private async Task<Provider> GetProviderOrThrowAsync(string providerId)
    {
        var provider = await _unitOfWork.Providers.GetByIdAsync(providerId);
        if (provider == null)
        {
            throw new NotFoundException($"Provider with ID {providerId} not found.");
        }

        return provider;
    }

    #endregion
}
