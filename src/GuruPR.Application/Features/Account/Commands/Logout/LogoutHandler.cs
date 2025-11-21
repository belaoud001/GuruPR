using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Features.Account.Exceptions;
using GuruPR.Application.Features.Users.Extensions;
using GuruPR.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Account.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly ILogger<LogoutHandler> _logger;
    private readonly ITokenService _tokenService;
    private readonly IHasher _hasher;
    private readonly IValidator<LogoutCommand> _validator;
    private readonly UserManager<User> _userManager;

    public LogoutHandler(ILogger<LogoutHandler> logger,
                                ITokenService tokenService,
                                IHasher hasher,
                                IValidator<LogoutCommand> validator,
                                UserManager<User> userManager)
    {
        _logger = logger;
        _tokenService = tokenService;
        _hasher = hasher;
        _validator = validator;
        _userManager = userManager;
    }


    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AccountValidationException(message, errors));

        var user = await _userManager.GetByIdOrThrowAsync(request.UserId);
        var isValidRefreshToken = _hasher.Verify(user.RefreshTokenHash, request.RefreshToken);
        if (!isValidRefreshToken || user.IsRefreshTokenExpired())
        {
            throw new RefreshTokenException("Invalid refresh token.");
        }

        user.ClearRefreshToken();

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            _logger.LogError("Failed to logout user with ID {UserId}. Errors: {Errors}", request.UserId, updateResult.Errors);

            throw new LogoutException("Failed to logout user.");
        }

        await _tokenService.RevokeTokensAsync(user);
    }
}
