using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Common.Markers.Interfaces;

using MediatR;

namespace GuruPR.Application.Common.Behaviors;

public class OwnershipValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                                              where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _provider;

    public OwnershipValidatorBehavior(IServiceProvider provider) { _provider = provider; }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Check if request implements IOwnedEntityRequest<TEntity> at runtime
        var ownedInterface = request.GetType()
                                    .GetInterfaces()
                                    .FirstOrDefault(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IOwnedEntityRequest<>));

        if (ownedInterface != null)
        {
            var entityType = ownedInterface.GetGenericArguments()[0];

            // Resolve the repository for that TEntity
            var repoType = typeof(IOwnedGenericRepository<>).MakeGenericType(entityType);
            dynamic repo = _provider.GetService(repoType) ?? throw new InvalidOperationException($"No repository for {entityType.Name}");

            // Perform the ownership check
            string id = ((dynamic)request).Id;
            string userId = ((dynamic)request).UserId;
            var entity = await repo.GetByIdAndOwnerAsync(id, userId);
            if (entity == null)
            {
                throw new UnauthorizedAccessException($"No access or not found for ID {id}");
            }
        }
        return await next();
    }
}
