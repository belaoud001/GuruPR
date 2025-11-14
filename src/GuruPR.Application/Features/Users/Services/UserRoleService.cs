using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Features.Users.Extensions;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Enums;
using GuruPR.Domain.Extensions.User;

using Microsoft.AspNetCore.Identity;

namespace GuruPR.Application.Features.Users.Services;

public class UserRoleService : IUserRoleService
{
    private readonly UserManager<User> _userManager;

    public UserRoleService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task AssignRoleAsync(string userId, UserRole userRole)
    {
        var user = await _userManager.GetByIdOrThrowAsync(userId);

        var result = await _userManager.AddToRoleAsync(user, userRole.ToName());
        if (!result.Succeeded)
        {
            throw new UserRoleOperationFailedException($"Failed to add role {userRole.ToName()} to user with email {user.Email}");
        }
    }

    public async Task RemoveRoleAsync(string userId, UserRole userRole)
    {
        var user = await _userManager.GetByIdOrThrowAsync(userId);

        var result = await _userManager.RemoveFromRoleAsync(user, userRole.ToName());
        if (!result.Succeeded)
        {
            throw new UserRoleOperationFailedException($"Failed to remove role {userRole.ToName()} from user with email {user.Email}");
        }
    }
}
