using Raga.Domain.Common;

namespace Raga.Domain.Tournaments;

public sealed class Tournament : BaseEntity
{
    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string RefNumber { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public long OwnerId { get; private set; }
    public int TournamentCupTypeId { get; private set; }

    // Required by EF Core
    private Tournament() { }

    private Tournament(
        string name,
        string refNumber,
        string? description,
        string? logoUrl,
        long ownerId,
        int tournamentCupTypeId)
    {
        Name = name;
        RefNumber = refNumber;
        Description = description;
        LogoUrl = logoUrl;
        OwnerId = ownerId;
        TournamentCupTypeId = tournamentCupTypeId;
    }

    /// <summary>
    /// Factory method — validates domain rules before constructing the entity.
    /// </summary>
    public static Result<Tournament> Create(
        string name,
        string refNumber,
        string? description,
        string? logoUrl,
        long ownerId,
        int tournamentCupTypeId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Tournament>.Failure("TOURNAMENT_NAME_REQUIRED", "Tournament name is required.");

        if (name.Length > 255)
            return Result<Tournament>.Failure("TOURNAMENT_NAME_TOO_LONG", "Tournament name must not exceed 255 characters.");

        if (string.IsNullOrWhiteSpace(refNumber))
            return Result<Tournament>.Failure("TOURNAMENT_REF_NUMBER_REQUIRED", "Reference number is required.");

        if (refNumber.Length > 100)
            return Result<Tournament>.Failure("TOURNAMENT_REF_NUMBER_TOO_LONG", "Reference number must not exceed 100 characters.");

        if (ownerId <= 0)
            return Result<Tournament>.Failure("TOURNAMENT_OWNER_INVALID", "Owner ID must be a positive number.");

        if (tournamentCupTypeId <= 0)
            return Result<Tournament>.Failure("TOURNAMENT_CUP_TYPE_INVALID", "Tournament Cup Type ID must be a positive number.");

        return Result<Tournament>.Success(new Tournament(
            name,
            refNumber,
            description,
            logoUrl,
            ownerId,
            tournamentCupTypeId));
    }
}
