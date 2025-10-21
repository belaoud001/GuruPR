using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using GuruPR.Domain.Entities;
using GuruPR.Persistence.Configuration.ContextConfiguration;
using GuruPR.Application.Interfaces.Infrastructure;

namespace GuruPR.Persistence.Contexts;

public class UserManagementDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options,
                                   ITokenEncryptionService tokenEncryptionService) : base(options)
    {
        _tokenEncryptionService = tokenEncryptionService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration(_tokenEncryptionService));
    }
}
