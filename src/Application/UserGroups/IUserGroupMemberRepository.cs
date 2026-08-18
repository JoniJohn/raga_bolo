using Raga.Domain.UserGroups;

namespace Raga.Application.UserGroups;

public interface IUserGroupMemberRepository
{
    Task<bool> GroupExistsAsync(long userGroupId, CancellationToken cancellationToken = default);
    Task<bool> IsMemberAsync(long userGroupId, long userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserGroupMember member, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
