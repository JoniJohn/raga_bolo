using Raga.Application.UserGroups;
using Raga.Domain.UserGroups;

namespace Raga.Infrastructure.Persistence.Repositories;

public sealed class UserGroupRepository(AppDbContext db) : IUserGroupRepository
{
    public async Task AddAsync(UserGroup userGroup, CancellationToken cancellationToken = default) =>
        await db.UserGroups.AddAsync(userGroup, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
