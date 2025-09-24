using Microsoft.EntityFrameworkCore;

using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Persistence.Contexts;

public class GuruDBContext : DbContext
{
    public DbSet<Provider> Providers { get; set; }


    public GuruDBContext(DbContextOptions<GuruDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
