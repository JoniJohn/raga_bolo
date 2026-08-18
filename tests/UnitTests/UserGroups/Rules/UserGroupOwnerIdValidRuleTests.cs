using FluentAssertions;
using Raga.Domain.UserGroups.Rules;
using Xunit;

namespace Raga.UnitTests.UserGroups.Rules;

public sealed class UserGroupOwnerIdValidRuleTests
{
    // ── IsBroken ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsBroken_WhenOwnerIdIsZero_ReturnsTrue()
    {
        new UserGroupOwnerIdValidRule(0).IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(long.MinValue)]
    public void IsBroken_WhenOwnerIdIsNegative_ReturnsTrue(long ownerId)
    {
        new UserGroupOwnerIdValidRule(ownerId).IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(long.MaxValue)]
    public void IsBroken_WhenOwnerIdIsPositive_ReturnsFalse(long ownerId)
    {
        new UserGroupOwnerIdValidRule(ownerId).IsBroken().Should().BeFalse();
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void ErrorCode_IsCorrectConstant()
    {
        new UserGroupOwnerIdValidRule(0).ErrorCode
            .Should().Be("USER_GROUP_OWNER_INVALID");
    }

    [Fact]
    public void Message_IsHumanReadable()
    {
        new UserGroupOwnerIdValidRule(0).Message
            .Should().Be("Owner ID must be a positive number.");
    }
}
