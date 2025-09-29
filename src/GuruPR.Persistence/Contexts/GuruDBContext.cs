using Microsoft.EntityFrameworkCore;

using GuruPR.Domain.Entities.OAuth;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Persistence.Configuration;

namespace GuruPR.Persistence.Contexts;

public class GuruDBContext : DbContext
{
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public DbSet<Provider> Providers { get; set; } = null!;

    public GuruDBContext(DbContextOptions<GuruDBContext> options, ITokenEncryptionService tokenEncryptionService) : base(options)
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

        modelBuilder.ApplyConfiguration(new ProviderConfiguration(_tokenEncryptionService));
    }
}
