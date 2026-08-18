using FluentAssertions;
using Raga.Domain.UserGroups.Rules;
using Xunit;

namespace Raga.UnitTests.UserGroups.Rules;

public sealed class UserGroupMemberUserIdValidRuleTests
{
    // ── IsBroken ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsBroken_WhenUserIdIsZero_ReturnsTrue()
    {
        new UserGroupMemberUserIdValidRule(0).IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(long.MinValue)]
    public void IsBroken_WhenUserIdIsNegative_ReturnsTrue(long userId)
    {
        new UserGroupMemberUserIdValidRule(userId).IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(long.MaxValue)]
    public void IsBroken_WhenUserIdIsPositive_ReturnsFalse(long userId)
    {
        new UserGroupMemberUserIdValidRule(userId).IsBroken().Should().BeFalse();
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void ErrorCode_IsCorrectConstant()
    {
        new UserGroupMemberUserIdValidRule(0).ErrorCode
            .Should().Be("USER_GROUP_MEMBER_USER_ID_INVALID");
    }

    [Fact]
    public void Message_IsHumanReadable()
    {
        new UserGroupMemberUserIdValidRule(0).Message
            .Should().Be("User ID must be a positive number.");
    }
}
