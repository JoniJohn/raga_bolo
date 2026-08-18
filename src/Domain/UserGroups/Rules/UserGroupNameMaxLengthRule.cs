using Raga.Domain.Common;

namespace Raga.Domain.UserGroups.Rules;

public sealed class UserGroupNameMaxLengthRule(string name) : IBusinessRule
{
    private const int MaxLength = 200;

    public string ErrorCode => "USER_GROUP_NAME_TOO_LONG";
    public string Message => $"Name must not exceed {MaxLength} characters.";
    public bool IsBroken() => name.Length > MaxLength;
}
