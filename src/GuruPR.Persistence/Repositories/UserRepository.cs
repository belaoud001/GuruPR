using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(UserManagementDbContext userManagementDbContext) : base(userManagementDbContext)
    {
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var user = await _dbSet.FirstOrDefaultAsync<User>(user => user.RefreshToken == refreshToken);

        return user;
    }
}
