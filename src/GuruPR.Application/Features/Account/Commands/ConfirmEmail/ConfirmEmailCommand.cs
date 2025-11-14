using MediatR;

namespace GuruPR.Application.Features.Account.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest;
