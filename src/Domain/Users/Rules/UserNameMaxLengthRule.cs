using Raga.Domain.Common;

namespace Raga.Domain.Users.Rules;

public sealed class UserNameMaxLengthRule(string name) : IBusinessRule
{
    private const int MaxLength = 255;

    public string ErrorCode => "USER_NAME_TOO_LONG";
    public string Message => $"Name must not exceed {MaxLength} characters.";
    public bool IsBroken() => name.Length > MaxLength;
}
