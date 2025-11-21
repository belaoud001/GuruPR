using MediatR;

namespace GuruPR.Application.Features.Account.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest;
