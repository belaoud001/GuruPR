using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Features.Account.Exceptions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.Google.GoogleCallback;

public class GoogleCallbackHandler : IRequestHandler<GoogleCallbackCommand, string>
{
    private readonly ILogger<GoogleCallbackHandler> _logger;
    private readonly IValidator<GoogleCallbackCommand> _validator;
    private readonly IExternalAuthService _externalAuthService;

    public GoogleCallbackHandler(ILogger<GoogleCallbackHandler> logger,
                                 IValidator<GoogleCallbackCommand> validator,
                                 IExternalAuthService externalAuthService)
    {
        _logger = logger;
        _validator = validator;
        _externalAuthService = externalAuthService;
    }

    public async Task<string> Handle(GoogleCallbackCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new ExternalLoginValidationException(message, errors));

        return await _externalAuthService.HandleGoogleCallbackAsync(request.ReturnUrl);
    }
}
