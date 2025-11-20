using GuruPR.Application.Common.Markers.Interfaces;

using MediatR;

namespace GuruPR.Application.Features.Account.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest, IUserContextCommand
{
    public string UserId { get; set; } = null!;
}
