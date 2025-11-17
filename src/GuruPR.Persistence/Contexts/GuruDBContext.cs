using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Domain.Entities.Agents;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Entities.Message;
using GuruPR.Domain.Entities.Provider;
using GuruPR.Domain.Entities.Tool;
using GuruPR.Persistence.Extensions.ContextConfiguration;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Contexts;

public class GuruDbContext : DbContext
{
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public DbSet<Tool> Tools { get; set; } = null!;
    public DbSet<Agent> Agents { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Provider> Providers { get; set; } = null!;
    public DbSet<Conversation> Conversations { get; set; } = null!;

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

        modelBuilder.ApplyConfiguration(new ToolConfiguration());
        modelBuilder.ApplyConfiguration(new AgentConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new ConversationConfiguration());
        modelBuilder.ApplyConfiguration(new ProviderConfiguration());
        modelBuilder.ApplyConfiguration(new ProviderConnectionConfiguration(_tokenEncryptionService));
    }
}
