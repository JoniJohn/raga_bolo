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

        // Static configuration seed data with static timestamp to prevent dynamic model change warnings
        builder.HasData(
            new
            {
                Id = 1,
                Code = "SL",
                Name = "Single Fixture League",
                Description = "Single fixture league tournament format",
                IsActive = true,
                CreatedAt = new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(5307), TimeSpan.Zero)
            },
            new
            {
                Id = 2,
                Code = "HAD",
                Name = "Home and Away League",
                Description = "Home and away league tournament format",
                IsActive = true,
                CreatedAt = new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(9364), TimeSpan.Zero)
            },
            new
            {
                Id = 3,
                Code = "SK",
                Name = "Single Fixture Knockout",
                Description = "Single fixture knockout tournament format",
                IsActive = true,
                CreatedAt = new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(9369), TimeSpan.Zero)
            },
            new
            {
                Id = 4,
                Code = "HAK",
                Name = "Home and Away Knockout",
                Description = "Home and away knockout tournament format",
                IsActive = true,
                CreatedAt = new DateTimeOffset(new DateTime(2026, 8, 28, 14, 32, 47, 612, DateTimeKind.Unspecified).AddTicks(9371), TimeSpan.Zero)
            }
        );
    }
}
