using AutoMapper;

using GuruPR.Application.Common.Interfaces.Presentation;
using GuruPR.Application.Features.Users.Dtos;
using GuruPR.Application.Features.Users.Extensions;
using GuruPR.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace GuruPR.Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<User> _userManager;

    public GetCurrentUserHandler(IMapper mapper,
                                 ICurrentUserService currentUserService,
                                 UserManager<User> userManager)
    {
        _mapper = mapper;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var user = await _userManager.GetByIdOrThrowAsync(currentUserId, cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}
