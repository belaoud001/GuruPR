using FluentValidation;

using GuruPR.Application.Common.Constants;
using GuruPR.Application.Common.Extensions;
using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Account.Exceptions;
using GuruPR.Application.Features.Users.Exceptions;
using GuruPR.Application.Features.Users.Extensions;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Enums;
using GuruPR.Domain.Extensions.User;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Features.Account.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand>
{
    private readonly ILogger<RegisterHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RegisterCommand> _validator;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IAccountLinkGenerator _accountLinkGenerator;
    private readonly UserManager<User> _userManager;

    public RegisterHandler(ILogger<RegisterHandler> logger,
                                  IUnitOfWork unitOfWork,
                                  IValidator<RegisterCommand> validator,
                                  IEmailSender emailSender,
                                  IEmailTemplateService emailTemplateService,
                                  IAccountLinkGenerator accountLinkGenerator,
                                  UserManager<User> userManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _emailSender = emailSender;
        _emailTemplateService = emailTemplateService;
        _accountLinkGenerator = accountLinkGenerator;
        _userManager = userManager;
    }

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await _validator.ThrowIfInvalidAsync(request,
                                             (message, errors) => new AccountValidationException(message, errors));

        await EnsureUserDoesNotExistAsync(request.Email);

        await _unitOfWork.BeginUserManagementTransactionAsync(cancellationToken);

        User user;

        try
        {
            user = await CreateUserAsync(request);

            await AssignRoleAsync(user.Id.ToString(), UserRole.User);

            await _unitOfWork.CommitUserManagementTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackUserManagementTransactionAsync(cancellationToken);

            throw;
        }

        try
        {
            await SendConfirmationEmailAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to user with email {Email}", user.Email);

            throw new EmailSendFailedException("Registration succeeded, but failed to send confirmation email. Please contact support.");
        }
    }

    private async Task EnsureUserDoesNotExistAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            throw new UserAlreadyExistsException($"User with email '{email}' already exists.");
        }
    }

    private async Task<User> CreateUserAsync(RegisterCommand request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            result.Errors.ThrowValidationException<RegistrationValidationException>("User registration failed.");
        }

        return user;
    }

    private async Task SendConfirmationEmailAsync(User user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = _accountLinkGenerator.GenerateConfirmationLink(user.Id, token);
        var emailBody = _emailTemplateService.BuildConfirmationEmailBody(user.FirstName, confirmationLink);

        await _emailSender.SendEmailAsync(user.Email ?? throw new EmailSendFailedException("Failed to send confirmation email."),
                                          Constants.ConfirmationEmailTitle,
                                          emailBody);
    }

    private async Task AssignRoleAsync(string userId, UserRole userRole)
    {
        var user = await _userManager.GetByIdOrThrowAsync(userId);

        var result = await _userManager.AddToRoleAsync(user, userRole.ToName());
        if (!result.Succeeded)
        {
            throw new UserRoleOperationFailedException($"Failed to add role {userRole.ToName()} to user with email {user.Email}");
        }
    }
}
