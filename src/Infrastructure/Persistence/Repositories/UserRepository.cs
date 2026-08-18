using Microsoft.EntityFrameworkCore;
using Raga.Application.Users;
using Raga.Domain.Users;

namespace Raga.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<bool> AuthIdExistsAsync(Guid authId, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(u => u.AuthId == authId, cancellationToken);

    public Task<bool> ExistsAsync(long userId, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(u => u.Id == userId, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await db.Users.AddAsync(user, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
