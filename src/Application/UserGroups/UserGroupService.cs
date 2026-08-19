using Raga.Application.UserGroups.DTOs;
using Raga.Application.Users;
using Raga.Domain.Common;
using Raga.Domain.UserGroups;

namespace Raga.Application.UserGroups;

public sealed class UserGroupService(
    IUserGroupRepository userGroupRepository,
    IUserGroupMemberRepository memberRepository,
    IUserRepository userRepository) : IUserGroupService
{
    public async Task<Result<UserGroupResponse>> CreateAsync(
        CreateUserGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Domain validation
        var createResult = UserGroup.Create(request.Name!, request.OwnerId);
        if (!createResult.IsSuccess)
            return Result<UserGroupResponse>.Failure(createResult.ErrorCode, createResult.ErrorMessage);

        // 2. Cross-entity policy: owner must exist
        var ownerExists = await userRepository.ExistsAsync(request.OwnerId, cancellationToken);
        if (!ownerExists)
            return Result<UserGroupResponse>.Failure(
                "USER_GROUP_OWNER_NOT_FOUND",
                "Owner user does not exist.");

        var userGroup = createResult.Value!;

        // 3. Persist group
        await userGroupRepository.AddAsync(userGroup, cancellationToken);
        await userGroupRepository.SaveChangesAsync(cancellationToken);

        // 4. Seed owner as the first member in the same logical operation
        var ownerMember = UserGroupMember.Create(userGroup.Id, userGroup.OwnerId).Value!;
        await memberRepository.AddAsync(ownerMember, cancellationToken);
        await memberRepository.SaveChangesAsync(cancellationToken);

        return Result<UserGroupResponse>.Success(
            new UserGroupResponse(userGroup.Id, userGroup.Name, userGroup.OwnerId));
    }
}
