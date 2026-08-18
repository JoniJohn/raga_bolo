using Raga.Application.UserGroups.DTOs;
using Raga.Domain.Common;

namespace Raga.Application.UserGroups;

public interface IUserGroupMemberService
{
    Task<Result<UserGroupMemberResponse>> AddMemberAsync(
        long userGroupId,
        AddMemberRequest request,
        CancellationToken cancellationToken = default);
}
