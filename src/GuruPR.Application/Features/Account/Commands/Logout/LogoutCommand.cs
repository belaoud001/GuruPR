using GuruPR.Application.Common.Markers;

using MediatR;

namespace GuruPR.Application.Features.Account.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest, IUserContextCommand
{
    public string UserId { get; set; } = null!;
}
