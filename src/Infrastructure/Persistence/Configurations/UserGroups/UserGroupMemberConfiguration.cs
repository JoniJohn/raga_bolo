using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raga.Domain.UserGroups;

namespace Raga.Infrastructure.Persistence.Configurations.UserGroups;

public sealed class UserGroupMemberConfiguration : IEntityTypeConfiguration<UserGroupMember>
{
    public void Configure(EntityTypeBuilder<UserGroupMember> builder)
    {
        builder.ToTable("UserGroupMember", "usr");

        builder.HasKey(m => new { m.UserGroupId, m.UserId });

        builder.Property(m => m.UserGroupId).IsRequired();
        builder.Property(m => m.UserId).IsRequired();

        builder.HasOne<Raga.Domain.UserGroups.UserGroup>()
            .WithMany()
            .HasForeignKey(m => m.UserGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Raga.Domain.Users.User>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
