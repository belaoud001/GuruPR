using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Domain.Entities.OAuth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Extensions.ContextConfiguration;

public class ProviderConnectionConfiguration : IEntityTypeConfiguration<ProviderConnection>
{
    ITokenEncryptionService _tokenEncryptionService;

    public ProviderConnectionConfiguration(ITokenEncryptionService tokenEncryptionService)
    {
        _tokenEncryptionService = tokenEncryptionService;
    }

    public void Configure(EntityTypeBuilder<ProviderConnection> builder)
    {
        builder.ToContainer("provider_connections")
               .HasPartitionKey(providerConnection => providerConnection.Id)
               .HasNoDiscriminator();

        builder.Property(providerConnection => providerConnection.Id)
               .IsRequired();

        builder.Property(providerConnection => providerConnection.ClientSecret)
               .HasConversion(
                    plainText => _tokenEncryptionService.Encrypt(plainText),
                    cipherText => _tokenEncryptionService.Decrypt(cipherText)
                );

        builder.Property(providerConnection => providerConnection.AccessToken)
               .HasConversion(
                      plainText => _tokenEncryptionService.Encrypt(plainText),
                      cipherText => _tokenEncryptionService.Decrypt(cipherText)
                );

        builder.Property(providerConnection => providerConnection.RefreshToken)
               .HasConversion(
                      plainText => _tokenEncryptionService.Encrypt(plainText),
                      cipherText => _tokenEncryptionService.Decrypt(cipherText)
                );
    }
}
