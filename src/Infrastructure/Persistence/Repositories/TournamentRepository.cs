using Microsoft.EntityFrameworkCore;
using Raga.Application.Tournaments;
using Raga.Domain.Tournaments;

namespace Raga.Infrastructure.Persistence.Repositories;

public sealed class TournamentRepository(AppDbContext db) : ITournamentRepository
{
    public Task<bool> RefNumberExistsAsync(string refNumber, CancellationToken cancellationToken = default) =>
        db.Tournaments.AnyAsync(t => t.RefNumber == refNumber, cancellationToken);

    public Task<bool> TournamentCupTypeExistsAsync(int tournamentCupTypeId, CancellationToken cancellationToken = default) =>
        db.TournamentCupTypes.AnyAsync(t => t.Id == tournamentCupTypeId, cancellationToken);

    public async Task AddAsync(Tournament tournament, CancellationToken cancellationToken = default) =>
        await db.Tournaments.AddAsync(tournament, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
