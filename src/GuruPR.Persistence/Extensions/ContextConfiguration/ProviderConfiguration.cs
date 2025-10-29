using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Domain.Entities.OAuth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Extensions.ContextConfiguration;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    private readonly ITokenEncryptionService _tokenEncryptionService;

    public ProviderConfiguration(ITokenEncryptionService tokenEncryptionService)
    {
        _tokenEncryptionService = tokenEncryptionService;
    }

    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToContainer("providers")
               .HasPartitionKey(provider => provider.Id)
               .HasNoDiscriminator();

        builder.Property(provider => provider.Id)
               .IsRequired();

        builder.OwnsMany(provider => provider.ProviderConnections, navigationBuilder =>
        {
            navigationBuilder.Property(providerConnection => providerConnection.ClientSecret)
                .HasConversion(
                    plainText => _tokenEncryptionService.Encrypt(plainText),
                    cipherText => _tokenEncryptionService.Decrypt(cipherText));

            navigationBuilder.Property(providerConnection => providerConnection.AccessToken)
                .HasConversion(
                    plainText => _tokenEncryptionService.Encrypt(plainText),
                    cipherText => _tokenEncryptionService.Decrypt(cipherText));

            navigationBuilder.Property(providerConnection => providerConnection.RefreshToken)
                .HasConversion(
                    plainText => _tokenEncryptionService.Encrypt(plainText),
                    cipherText => _tokenEncryptionService.Decrypt(cipherText));
        });
    }
}
