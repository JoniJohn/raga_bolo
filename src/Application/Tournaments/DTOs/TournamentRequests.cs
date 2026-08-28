namespace Raga.Application.Tournaments.DTOs;

public sealed record CreateTournamentRequest(
    string? Name,
    string? RefNumber,
    string? Description,
    string? LogoUrl,
    long OwnerId,
    int TournamentCupTypeId);

public sealed record TournamentResponse(
    long Id,
    string Name,
    string RefNumber,
    string? Description,
    string? LogoUrl,
    long OwnerId,
    int TournamentCupTypeId,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
