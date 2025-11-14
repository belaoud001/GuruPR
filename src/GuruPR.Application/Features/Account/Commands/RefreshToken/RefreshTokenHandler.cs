
using FluentValidation;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Features.Account.Exceptions;
using GuruPR.Application.Features.Users.Extensions;
using GuruPR.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Account.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand>
{
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly IHasher _hasher;
    private readonly IValidator<RefreshTokenCommand> _validator;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public RefreshTokenHandler(ILogger<RefreshTokenHandler> logger,
                               IHasher hasher,
                               IValidator<RefreshTokenCommand> validator,
                               ITokenService tokenService,
                               UserManager<User> userManager)
    {
        _logger = logger;
        _hasher = hasher;
        _validator = validator;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AccountValidationException(message, errors));

        var user = await _userManager.GetByIdOrThrowAsync(request.UserId);

        var isValidRefreshTokenHash = _hasher.Verify(user.RefreshTokenHash, request.RefreshToken);
        if (!isValidRefreshTokenHash || user.IsRefreshTokenExpired())
        {
            _logger.LogError("Invalid or expired refresh token for user with ID {UserId}.", request.UserId);

            throw new RefreshTokenException("Refresh token has expired.");
        }

        await _tokenService.RenewAccessTokenAsync(user);
    }
}
