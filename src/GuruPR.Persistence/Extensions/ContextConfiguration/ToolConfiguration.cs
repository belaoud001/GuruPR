using GuruPR.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Extensions.ContextConfiguration;

public class ToolConfiguration : IEntityTypeConfiguration<Tool>
{
    public ToolConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<Tool> builder)
    {
        builder.ToContainer("tools")
               .HasPartitionKey(tool => tool.Id)
               .HasNoDiscriminator();

        builder.Property(tool => tool.Id);
    }
}
