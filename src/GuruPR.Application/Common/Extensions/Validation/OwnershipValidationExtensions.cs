using GuruPR.Application.Common.Behaviors;
using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Domain.Interfaces.Markers;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace GuruPR.Application.Common.Extensions.Validation;

public static class OwnershipValidationExtensions
{
    public static IServiceCollection AddOwnershipValidation<TRequest, TResponse, TEntity>(this IServiceCollection services)
           where TRequest : IOwnedEntityRequest<TEntity>, IRequest<TResponse>
           where TEntity : class, IOwnedEntity
    {
        services.AddTransient<IPipelineBehavior<TRequest, TResponse>, OwnershipValidatorBehavior<TRequest, TResponse>>();

        return services;
    }
}
