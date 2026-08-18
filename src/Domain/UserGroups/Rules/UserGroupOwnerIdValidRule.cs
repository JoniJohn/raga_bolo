using Raga.Domain.Common;

namespace Raga.Domain.UserGroups.Rules;

public sealed class UserGroupOwnerIdValidRule(long ownerId) : IBusinessRule
{
    public string ErrorCode => "USER_GROUP_OWNER_INVALID";
    public string Message => "Owner ID must be a positive number.";
    public bool IsBroken() => ownerId <= 0;
}
