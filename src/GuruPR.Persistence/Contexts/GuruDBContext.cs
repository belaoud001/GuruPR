using Microsoft.EntityFrameworkCore;

using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Persistence.Contexts;

public class GuruDBContext : DbContext
{
    public DbSet<Provider> Providers { get; set; } = null!;

    public GuruDBContext(DbContextOptions<GuruDBContext> options) : base(options)
    {
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

        modelBuilder.Entity<Provider>().ToContainer("providers")
                                       .HasPartitionKey(provider => provider.Id)
                                       .HasNoDiscriminator()
                                       .Property(provider => provider.Id)
                                       .IsRequired();
    }
}
