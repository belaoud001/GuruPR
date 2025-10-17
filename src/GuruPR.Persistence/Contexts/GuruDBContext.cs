using Microsoft.EntityFrameworkCore;

using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Persistence.Configuration.ContextConfiguration;

namespace GuruPR.Persistence.Contexts;

public class GuruDbContext : DbContext
{
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public DbSet<Provider> Providers { get; set; } = null!;

    public GuruDbContext(DbContextOptions<GuruDbContext> options, ITokenEncryptionService tokenEncryptionService) : base(options)
    {
        _tokenEncryptionService = tokenEncryptionService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

#if DEBUG
        optionsBuilder.EnableDetailedErrors(true);
        optionsBuilder.EnableSensitiveDataLogging(true);
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new GuruDbContextConfiguration(_tokenEncryptionService));
    }
}
