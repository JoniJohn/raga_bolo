using Raga.Application.Tournaments.DTOs;
using Raga.Application.Users;
using Raga.Domain.Common;
using Raga.Domain.Tournaments;

namespace Raga.Application.Tournaments;

public sealed class TournamentService(
    ITournamentRepository tournamentRepository,
    IUserRepository userRepository) : ITournamentService
{
    public async Task<Result<TournamentResponse>> CreateAsync(
        CreateTournamentRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Domain validation
        var createResult = Tournament.Create(
            request.Name!,
            request.RefNumber!,
            request.Description,
            request.LogoUrl,
            request.OwnerId,
            request.TournamentCupTypeId);

        if (!createResult.IsSuccess)
            return Result<TournamentResponse>.Failure(createResult.ErrorCode, createResult.ErrorMessage);

        // 2. Cross-entity policy: owner user must exist
        var ownerExists = await userRepository.ExistsAsync(request.OwnerId, cancellationToken);
        if (!ownerExists)
            return Result<TournamentResponse>.Failure(
                "USER_NOT_FOUND",
                $"User with ID {request.OwnerId} was not found.");

        // 3. Cross-entity policy: tournament cup type must exist
        var cupTypeExists = await tournamentRepository.TournamentCupTypeExistsAsync(request.TournamentCupTypeId, cancellationToken);
        if (!cupTypeExists)
            return Result<TournamentResponse>.Failure(
                "TOURNAMENT_CUP_TYPE_NOT_FOUND",
                $"Tournament cup type with ID {request.TournamentCupTypeId} was not found.");

        // 4. Cross-entity policy: reference number must be unique
        var refNumberExists = await tournamentRepository.RefNumberExistsAsync(request.RefNumber!, cancellationToken);
        if (refNumberExists)
            return Result<TournamentResponse>.Failure(
                "TOURNAMENT_REF_NUMBER_DUPLICATE",
                $"A tournament with reference number '{request.RefNumber}' already exists.");

        var tournament = createResult.Value!;

        // 5. Persist tournament
        await tournamentRepository.AddAsync(tournament, cancellationToken);
        await tournamentRepository.SaveChangesAsync(cancellationToken);

        return Result<TournamentResponse>.Success(new TournamentResponse(
            tournament.Id,
            tournament.Name,
            tournament.RefNumber,
            tournament.Description,
            tournament.LogoUrl,
            tournament.OwnerId,
            tournament.TournamentCupTypeId,
            tournament.IsActive,
            tournament.CreatedAt,
            tournament.UpdatedAt));
    }
}
