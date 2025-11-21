using MediatR;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin;

public record ExternalLoginCommand<T>(string? ReturnUrl) : IRequest<T>;
