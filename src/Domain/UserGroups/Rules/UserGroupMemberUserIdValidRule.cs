using Raga.Domain.Common;

namespace Raga.Domain.UserGroups.Rules;

public sealed class UserGroupMemberUserIdValidRule(long userId) : IBusinessRule
{
    public string ErrorCode => "USER_GROUP_MEMBER_USER_ID_INVALID";
    public string Message => "User ID must be a positive number.";
    public bool IsBroken() => userId <= 0;
}
