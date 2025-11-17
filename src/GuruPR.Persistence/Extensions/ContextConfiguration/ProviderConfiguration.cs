using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Domain.Entities.Provider;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Extensions.ContextConfiguration;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public ProviderConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToContainer("providers")
               .HasPartitionKey(provider => provider.Id)
               .HasNoDiscriminator();

        builder.Property(provider => provider.Id)
               .IsRequired();
    }
}
