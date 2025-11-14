using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Common.Markers;
using GuruPR.Domain.Interfaces.Markers;

using MediatR;

namespace GuruPR.Application.Common.Behaviors;

public class OwnershipValidatorBehavior<TRequest, TResponse, TEntity> : IPipelineBehavior<TRequest, TResponse>
                                                       where TRequest : IOwnedEntityRequest<TEntity>
                                                       where TEntity : class, IOwnedEntity
{
    private readonly IOwnedGenericRepository<TEntity> _ownedGenericRepository;

    public OwnershipValidatorBehavior(IOwnedGenericRepository<TEntity> ownedGenericRepository)
    {
        _ownedGenericRepository = ownedGenericRepository;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            throw new ArgumentException("Entity ID is required.");
        }

        if (string.IsNullOrEmpty(request.UserId))
        {
            throw new UnauthorizedAccessException("User context is required.");
        }

        var entity = await _ownedGenericRepository.GetByIdAndOwnerAsync(request.Id, request.UserId);

        if (entity == null)
        {
            throw new UnauthorizedAccessException($"Entity with ID '{request.Id}' not found or you do not have permission to access it.");
        }

        return await next();
    }
}
