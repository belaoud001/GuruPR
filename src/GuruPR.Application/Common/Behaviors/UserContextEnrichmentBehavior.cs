using GuruPR.Application.Common.Interfaces.Presentation;
using GuruPR.Application.Common.Markers;

using MediatR;

namespace GuruPR.Application.Common.Behaviors;

public sealed class UserContextEnrichmentBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                                                        where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;

    public UserContextEnrichmentBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IUserContextCommand userContextCommand)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("Authenticated user id is required but was not found.");
            }

            userContextCommand.UserId = userId;
        }

        return await next();
    }
}
