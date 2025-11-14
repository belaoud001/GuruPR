using FluentValidation;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Features.Account.Exceptions;
using GuruPR.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace GuruPR.Application.Features.Account.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand>
{
    private readonly ITokenService _tokenService;
    private readonly IValidator<LoginCommand> _validator;
    private readonly UserManager<User> _userManager;

    public LoginHandler(ITokenService tokenService, IValidator<LoginCommand> validator, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _validator = validator;
        _userManager = userManager;
    }

    public async Task Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AccountValidationException(message, errors));

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new LoginFailedException("Login failed. Invalid email or password.");
        }

        if (user.NeedsRefreshTokenRenewal())
        {
            await _tokenService.IssueNewTokenPairAsync(user);
        }
        else
        {
            await _tokenService.RenewAccessTokenAsync(user);
        }
    }
}
