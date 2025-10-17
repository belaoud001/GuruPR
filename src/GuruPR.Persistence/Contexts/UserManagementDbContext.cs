using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using GuruPR.Domain.Entities;
using GuruPR.Persistence.Configuration.ContextConfiguration;

namespace GuruPR.Persistence.Contexts;

public class UserManagementDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<User> Users { get; set; }

    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserManagementDbContextConfiguration());
    }
}
