using FluentAssertions;
using Raga.Domain.UserGroups.Rules;
using Xunit;

namespace Raga.UnitTests.UserGroups.Rules;

public sealed class UserGroupNameMaxLengthRuleTests
{
    private const int MaxLength = 200;

    // ── IsBroken ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsBroken_WhenNameIsExactlyMaxLength_ReturnsFalse()
    {
        var rule = new UserGroupNameMaxLengthRule(new string('A', MaxLength));
        rule.IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WhenNameIsOneLongerThanMaxLength_ReturnsTrue()
    {
        var rule = new UserGroupNameMaxLengthRule(new string('A', MaxLength + 1));
        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenNameIsWellWithinLimit_ReturnsFalse()
    {
        new UserGroupNameMaxLengthRule("Tournament Admins").IsBroken().Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WhenNameIsSignificantlyOverLimit_ReturnsTrue()
    {
        var rule = new UserGroupNameMaxLengthRule(new string('X', MaxLength + 100));
        rule.IsBroken().Should().BeTrue();
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void ErrorCode_IsCorrectConstant()
    {
        new UserGroupNameMaxLengthRule(string.Empty).ErrorCode
            .Should().Be("USER_GROUP_NAME_TOO_LONG");
    }

    [Fact]
    public void Message_ContainsMaxLength()
    {
        new UserGroupNameMaxLengthRule(string.Empty).Message
            .Should().Contain("200");
    }
}
