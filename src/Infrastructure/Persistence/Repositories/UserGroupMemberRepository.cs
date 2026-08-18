using Microsoft.EntityFrameworkCore;
using Raga.Application.UserGroups;
using Raga.Domain.UserGroups;

namespace Raga.Infrastructure.Persistence.Repositories;

public sealed class UserGroupMemberRepository(AppDbContext db) : IUserGroupMemberRepository
{
    public Task<bool> GroupExistsAsync(long userGroupId, CancellationToken cancellationToken = default) =>
        db.UserGroups.AnyAsync(g => g.Id == userGroupId, cancellationToken);

    public Task<bool> IsMemberAsync(long userGroupId, long userId, CancellationToken cancellationToken = default) =>
        db.UserGroupMembers.AnyAsync(m => m.UserGroupId == userGroupId && m.UserId == userId, cancellationToken);

    public async Task AddAsync(UserGroupMember member, CancellationToken cancellationToken = default) =>
        await db.UserGroupMembers.AddAsync(member, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
