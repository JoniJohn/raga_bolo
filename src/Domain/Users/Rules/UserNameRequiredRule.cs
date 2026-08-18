using Raga.Domain.Common;

namespace Raga.Domain.Users.Rules;

public sealed class UserNameRequiredRule(string name) : IBusinessRule
{
    public string ErrorCode => "USER_NAME_REQUIRED";
    public string Message => "Name is required.";
    public bool IsBroken() => string.IsNullOrWhiteSpace(name);
}
