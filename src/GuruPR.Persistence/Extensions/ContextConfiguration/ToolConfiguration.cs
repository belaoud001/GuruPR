using System.Text.Json;

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

        builder.Property(tool => tool.Config)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null)
            );

        builder.Property(tool => tool.ParametersSchema)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null)
            );
    }
}
