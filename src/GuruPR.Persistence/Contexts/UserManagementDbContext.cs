using GuruPR.Domain.Entities;
using GuruPR.Persistence.Configuration.ContextConfiguration;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Contexts;

public class UserManagementDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
