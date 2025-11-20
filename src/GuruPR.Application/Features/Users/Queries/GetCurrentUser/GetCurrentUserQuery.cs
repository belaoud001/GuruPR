using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Application.Features.Users.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<UserDto>, IUserContextCommand
{
    public string UserId { get; set; } = null!;
}
