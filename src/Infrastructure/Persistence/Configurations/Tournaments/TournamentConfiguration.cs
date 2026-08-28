using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raga.Domain.Tournaments;
using Raga.Domain.Users;

namespace Raga.Infrastructure.Persistence.Configurations.Tournaments;

public sealed class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.ToTable("Tournament", "tour");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .UseIdentityAlwaysColumn();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.RefNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.RefNumber)
            .IsUnique();

        builder.Property(t => t.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(t => t.LogoUrl)
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TournamentCupType>()
            .WithMany()
            .HasForeignKey(t => t.TournamentCupTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
