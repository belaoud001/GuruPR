using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Features.Account.Exceptions;
using GuruPR.Application.Features.Users.Extensions;
using GuruPR.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Account.Commands.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand>
{
    private readonly ILogger<ConfirmEmailHandler> _logger;
    private readonly IValidator<ConfirmEmailCommand> _validator;
    private readonly UserManager<User> _userManager;

    public ConfirmEmailHandler(ILogger<ConfirmEmailHandler> logger,
                               IValidator<ConfirmEmailCommand> validator,
                               UserManager<User> userManager)
    {
        _logger = logger;
        _validator = validator;
        _userManager = userManager;
    }

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AccountValidationException(message, errors));
        var user = await _userManager.GetByIdOrThrowAsync(request.UserId);

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            _logger.LogError("Email confirmation failed for user with ID {UserId}. Errors: {Errors}", request.UserId,
                              string.Join(", ", result.Errors.Select(e => e.Description)));

            throw new EmailConfirmationException("Email confirmation failed.");
        }
    }
}
