using System.Text.Json;

using GuruPR.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Extensions.ContextConfiguration;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public ConversationConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToContainer("conversations")
               .HasPartitionKey(conversation => conversation.Id)
               .HasNoDiscriminator();

        builder.Property(conversation => conversation.Id)
               .IsRequired();

        builder.OwnsOne(conversation => conversation.Metadata, modelConfig =>
        {
            modelConfig.Property(mc => mc.CustomData)
                       .HasConversion(
                           v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                           v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null)
                       );
        });
    }
}
