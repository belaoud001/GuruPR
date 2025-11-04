using GuruPR.Domain.Entities.Message;
using GuruPR.Persistence.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Extensions.ContextConfiguration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public MessageConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToContainer("messages")
               .HasPartitionKey(message => message.Id)
               .HasNoDiscriminator();

        builder.Property(message => message.Id)
               .IsRequired();

        builder.OwnsOne(message => message.MetaData, modelConfig =>
        {
            modelConfig.Property(mc => mc.CustomData)
                       .HasConversion(new DictionaryJsonConverter());
        });
    }
}
