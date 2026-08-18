using Raga.Application.UserGroups.DTOs;
using Raga.Application.Users;
using Raga.Domain.Common;
using Raga.Domain.UserGroups;
using Raga.Domain.UserGroups.Rules;

namespace Raga.Application.UserGroups;

public sealed class UserGroupMemberService(
    IUserGroupMemberRepository memberRepository,
    IUserRepository userRepository) : IUserGroupMemberService
{
    public async Task<Result<UserGroupMemberResponse>> AddMemberAsync(
        long userGroupId,
        AddMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Domain validation — userId must be positive
        var userIdValidRule = new UserGroupMemberUserIdValidRule(request.UserId);
        if (userIdValidRule.IsBroken())
            return Result<UserGroupMemberResponse>.Failure(
                userIdValidRule.ErrorCode, userIdValidRule.Message);

        // 2. Cross-entity policy: group must exist
        var groupExists = await memberRepository.GroupExistsAsync(userGroupId, cancellationToken);
        if (!groupExists)
            return Result<UserGroupMemberResponse>.Failure(
                "USER_GROUP_NOT_FOUND",
                "User group does not exist.");

        // 3. Cross-entity policy: user must exist
        var userExists = await userRepository.ExistsAsync(request.UserId, cancellationToken);
        if (!userExists)
            return Result<UserGroupMemberResponse>.Failure(
                "USER_NOT_FOUND",
                "User does not exist.");

        // 4. Cross-entity policy: must not already be a member
        var alreadyMember = await memberRepository.IsMemberAsync(userGroupId, request.UserId, cancellationToken);
        if (alreadyMember)
            return Result<UserGroupMemberResponse>.Failure(
                "USER_GROUP_MEMBER_DUPLICATE",
                "User is already a member of this group.");

        // 5. Persist
        var member = new UserGroupMember(userGroupId, request.UserId);
        await memberRepository.AddAsync(member, cancellationToken);
        await memberRepository.SaveChangesAsync(cancellationToken);

        return Result<UserGroupMemberResponse>.Success(
            new UserGroupMemberResponse(userGroupId, request.UserId));
    }
}
