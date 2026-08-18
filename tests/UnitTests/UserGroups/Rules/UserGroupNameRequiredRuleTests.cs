using FluentAssertions;
using Raga.Domain.UserGroups.Rules;
using Xunit;

namespace Raga.UnitTests.UserGroups.Rules;

public sealed class UserGroupNameRequiredRuleTests
{
    // ── IsBroken ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsBroken_WhenNameIsNull_ReturnsTrue()
    {
        var rule = new UserGroupNameRequiredRule(null!);
        rule.IsBroken().Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenNameIsEmpty_ReturnsTrue()
    {
        var rule = new UserGroupNameRequiredRule(string.Empty);
        rule.IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void IsBroken_WhenNameIsWhitespaceOnly_ReturnsTrue(string name)
    {
        var rule = new UserGroupNameRequiredRule(name);
        rule.IsBroken().Should().BeTrue();
    }

    [Theory]
    [InlineData("Tournament Admins")]
    [InlineData("A")]
    [InlineData(" Leading space")]
    public void IsBroken_WhenNameHasContent_ReturnsFalse(string name)
    {
        var rule = new UserGroupNameRequiredRule(name);
        rule.IsBroken().Should().BeFalse();
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void ErrorCode_IsCorrectConstant()
    {
        new UserGroupNameRequiredRule(string.Empty).ErrorCode
            .Should().Be("USER_GROUP_NAME_REQUIRED");
    }

    [Fact]
    public void Message_IsHumanReadable()
    {
        new UserGroupNameRequiredRule(string.Empty).Message
            .Should().Be("Name is required.");
    }
}
