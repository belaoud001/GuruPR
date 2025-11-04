using System.Text.Json;

using GuruPR.Domain.Entities.Tool;
using GuruPR.Persistence.Helpers;

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

        var jsonOptions = new JsonSerializerOptions
        {
        };

        builder.Property(tool => tool.Config)
               .HasConversion(new DictionaryJsonConverter());

        builder.Property(tool => tool.ParametersSchema)
               .HasConversion(new DictionaryJsonConverter());
    }
}
