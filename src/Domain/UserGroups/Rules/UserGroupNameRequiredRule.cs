using Raga.Domain.Common;

namespace Raga.Domain.UserGroups.Rules;

public sealed class UserGroupNameRequiredRule(string name) : IBusinessRule
{
    public string ErrorCode => "USER_GROUP_NAME_REQUIRED";
    public string Message => "Name is required.";
    public bool IsBroken() => string.IsNullOrWhiteSpace(name);
}
