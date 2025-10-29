using GuruPR.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Extensions.ContextConfiguration;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public AgentConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToContainer("agents")
               .HasPartitionKey(agent => agent.Id)
               .HasNoDiscriminator();

        builder.Property(agent => agent.Id)
               .IsRequired();
    }
}
