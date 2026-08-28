using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raga.Domain.Tournaments;

namespace Raga.Infrastructure.Persistence.Configurations.Tournaments;

public sealed class TournamentCupTypeConfiguration : IEntityTypeConfiguration<TournamentCupType>
{
    public void Configure(EntityTypeBuilder<TournamentCupType> builder)
    {
        builder.ToTable("TournamentCupType", "conf");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(t => t.Code)
            .IsUnique();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        // Static configuration seed data
        builder.HasData(
            new TournamentCupType(1, "SL", "Single Fixture League", "Single fixture league tournament format"),
            new TournamentCupType(2, "HAD", "Home and Away League", "Home and away league tournament format"),
            new TournamentCupType(3, "SK", "Single Fixture Knockout", "Single fixture knockout tournament format"),
            new TournamentCupType(4, "HAK", "Home and Away Knockout", "Home and away knockout tournament format")
        );
    }
}
