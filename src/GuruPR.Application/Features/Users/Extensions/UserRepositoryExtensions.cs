using GuruPR.Application.Features.Users.Exceptions;
using GuruPR.Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace GuruPR.Application.Features.Users.Extensions;

public static class UserRepositoryExtensions
{
    public static async Task<User> GetByIdOrThrowAsync(this UserManager<User> userManager,
                                                            string id,
                                                            CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(id);

        return user ?? throw new UserNotFoundException($"User with the given ID {id} not found.");
    }
}
