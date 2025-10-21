using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GuruPR.Persistence.Configuration.ContextConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    ITokenEncryptionService _tokenEncryptionService;

    public UserConfiguration(ITokenEncryptionService tokenEncryptionService)
    {
        _tokenEncryptionService = tokenEncryptionService;
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.FirstName)
               .HasMaxLength(256);

        builder.Property(user => user.LastName)
               .HasMaxLength(256);

        builder.Property(user => user.RefreshToken)
               .HasConversion(
                   plaintText => plaintText == null ? null : _tokenEncryptionService.Encrypt(plaintText),
                   cipherText => cipherText == null ? null : _tokenEncryptionService.Decrypt(cipherText)
               );
    }
}
