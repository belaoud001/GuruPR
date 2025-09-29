using AutoMapper;

using GuruPR.Domain.Exceptions;
using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Exceptions;
using GuruPR.Application.Dtos.OAuth.Provider;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Application.Interfaces.Application;

namespace GuruPR.Application.Services.OAuth;

/// <summary>
/// Service for managing OAuth providers & provider connection, 
/// including creation, retrieval, updating, and deletion (CRUD - Operations).
/// </summary>
public class ProviderService : IProviderService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;


    public ProviderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    #region Public Methods

    /// <summary>
    /// Creates a new OAuth provider.
    /// </summary>
    /// <param name="createProviderRequest">Request containing provider details.</param>
    /// <returns>The created provider as a DTO.</returns>
    /// <exception cref="DomainException">Thrown when the provider configuration is invalid.</exception>
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

    /// <summary>
    /// Retrieves all OAuth providers.
    /// </summary>
    /// <returns>A collection of provider DTOs.</returns>
    public async Task<IEnumerable<ProviderDto>> GetAllProvidersAsync()
    {
        var providers = await _unitOfWork.Providers.GetAllAsync();

        return _mapper.Map<IEnumerable<ProviderDto>>(providers);
    }

    /// <summary>
    /// Retrieves an OAuth provider by its ID.
    /// </summary>
    /// <param name="providerId">The ID of the provider.</param>
    /// <returns>The provider as a DTO.</returns>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<ProviderDto> GetProviderByIdAsync(string providerId)
    {
        var provider = await GetProviderOrThrowAsync(providerId);

        return _mapper.Map<ProviderDto>(provider);
    }

    /// <summary>
    /// Updates an existing OAuth provider.
    /// </summary>
    /// <param name="providerId">The ID of the provider to update.</param>
    /// <param name="updateProviderRequest">Request containing updated provider details.</param>
    /// <returns>The updated provider as a DTO.</returns>
    /// <exception cref="DomainException">Thrown when the updated provider configuration is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<ProviderDto> UpdateProviderAsync(string providerId, UpdateProviderRequest updateProviderRequest)
    {
        var provider = await GetProviderOrThrowAsync(providerId);

        _mapper.Map(updateProviderRequest, provider);

        if (!provider.IsValid())
        {
            throw new DomainException("Invalid provider configuration, please recheck provider configuration.");
        }

        _unitOfWork.Providers.Update(provider);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProviderDto>(provider);
    }

    /// <summary>
    /// Deletes an OAuth provider by its ID.
    /// </summary>
    /// <param name="providerId">The ID of the provider to delete.</param>
    /// <returns>True if the provider was successfully deleted; otherwise, false.</returns>
    /// <exception cref="NotFoundException">Thrown when the provider is not found.</exception>
    public async Task<bool> DeleteProviderAsync(string providerId)
    {
        var provider = await GetProviderOrThrowAsync(providerId);

        _unitOfWork.Providers.Delete(provider);

        var result = await _unitOfWork.SaveChangesAsync();
        return result > 0;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Retrieves an OAuth provider by its ID or throws an exception if not found.
    /// </summary>
    /// <param name="providerId">The ID of the provider.</param>
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
