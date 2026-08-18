using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raga.Domain.UserGroups;

namespace Raga.Infrastructure.Persistence.Configurations.UserGroups;

public sealed class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.ToTable("UserGroup", "usr");

        builder.HasKey(ug => ug.Id);

        builder.Property(ug => ug.Id)
            .UseIdentityAlwaysColumn();

        builder.Property(ug => ug.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ug => ug.OwnerId)
            .IsRequired();

        builder.HasOne<Raga.Domain.Users.User>()
            .WithMany()
            .HasForeignKey(ug => ug.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
