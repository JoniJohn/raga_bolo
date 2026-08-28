using Raga.Domain.Tournaments;

namespace Raga.Application.Tournaments;

public interface ITournamentRepository
{
    Task<bool> RefNumberExistsAsync(string refNumber, CancellationToken cancellationToken = default);
    Task<bool> TournamentCupTypeExistsAsync(int tournamentCupTypeId, CancellationToken cancellationToken = default);
    Task AddAsync(Tournament tournament, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
