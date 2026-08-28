using Raga.Application.Tournaments.DTOs;
using Raga.Domain.Common;

namespace Raga.Application.Tournaments;

public interface ITournamentService
{
    Task<Result<TournamentResponse>> CreateAsync(
        CreateTournamentRequest request,
        CancellationToken cancellationToken = default);
}
