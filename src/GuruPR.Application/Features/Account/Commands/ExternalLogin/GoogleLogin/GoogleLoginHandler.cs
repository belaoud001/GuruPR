using FluentValidation;

using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Features.Account.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.GoogleLogin;

public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, ChallengeResult>
{
    private readonly ILogger<GoogleLoginHandler> _logger;
    private readonly IValidator<GoogleLoginCommand> _validator;
    private readonly IExternalAuthService _externalAuthService;

    public GoogleLoginHandler(ILogger<GoogleLoginHandler> logger,
                              IValidator<GoogleLoginCommand> validator,
                              IExternalAuthService externalAuthService)
    {
        _logger = logger;
        _validator = validator;
        _externalAuthService = externalAuthService;
    }


    public async Task<ChallengeResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ExternalLoginValidationException(message, errors));

        return await _externalAuthService.InitiateGoogleLoginAsync(request.ReturnUrl);
    }
}
