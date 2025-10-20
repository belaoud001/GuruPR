using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using GuruPR.Domain.Entities;

namespace GuruPR.Persistence.Configuration.ContextConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public UserConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.FirstName)
               .HasMaxLength(256);

        builder.Property(user => user.LastName)
               .HasMaxLength(256);
    }
}
