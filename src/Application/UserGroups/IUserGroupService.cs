using Raga.Application.UserGroups.DTOs;
using Raga.Domain.Common;

namespace Raga.Application.UserGroups;

public interface IUserGroupService
{
    Task<Result<UserGroupResponse>> CreateAsync(CreateUserGroupRequest request, CancellationToken cancellationToken = default);
}
