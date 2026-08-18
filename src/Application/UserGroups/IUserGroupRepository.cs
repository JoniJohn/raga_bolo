using Raga.Domain.UserGroups;

namespace Raga.Application.UserGroups;

public interface IUserGroupRepository
{
    Task AddAsync(UserGroup userGroup, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
