using Raga.Domain.Common;

namespace Raga.Domain.UserGroups;

public sealed class UserGroupMember : BaseEntity
{
    public long UserGroupId { get; private set; }
    public long UserId { get; private set; }

    // Required by EF Core
    private UserGroupMember() { }

    private UserGroupMember(long userGroupId, long userId)
    {
        UserGroupId = userGroupId;
        UserId = userId;
    }

    /// <summary>
    /// Factory method — validates domain rules before constructing the entity.
    /// Group-exists and user-exists checks are cross-entity policies handled at the service layer.
    /// </summary>
    public static Result<UserGroupMember> Create(long userGroupId, long userId)
    {
        if (userId <= 0)
            return Result<UserGroupMember>.Failure("USER_GROUP_MEMBER_USER_ID_INVALID", "User ID must be a positive number.");

        return Result<UserGroupMember>.Success(new UserGroupMember(userGroupId, userId));
    }
}
