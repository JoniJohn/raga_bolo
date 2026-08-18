namespace Raga.Domain.UserGroups;

public sealed class UserGroupMember
{
    public long UserGroupId { get; private set; }
    public long UserId { get; private set; }

    // Required by EF Core
    private UserGroupMember() { }

    public UserGroupMember(long userGroupId, long userId)
    {
        UserGroupId = userGroupId;
        UserId = userId;
    }
}
