using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raga.Domain.Users;

namespace Raga.Infrastructure.Persistence.Configurations.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", "usr");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .UseIdentityAlwaysColumn();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.AuthId)
            .IsRequired(false);

        builder.HasIndex(u => u.AuthId)
            .IsUnique()
            .HasFilter("\"AuthId\" IS NOT NULL");
    }
}
