using MediatR;

namespace GuruPR.Application.Features.Account.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password) : IRequest;

