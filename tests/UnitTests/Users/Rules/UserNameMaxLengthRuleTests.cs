using FluentAssertions;
using Raga.Domain.Users.Rules;
using Xunit;

namespace Raga.UnitTests.Users.Rules;

public sealed class UserNameMaxLengthRuleTests
{
    private const int MaxLength = 255;

    // ── IsBroken ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsBroken_WhenNameIsExactlyMaxLength_ReturnsFalse()
    {
        // Arrange
        var name = new string('A', MaxLength);
        var rule = new UserNameMaxLengthRule(name);

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WhenNameIsOneLongerThanMaxLength_ReturnsTrue()
    {
        // Arrange
        var name = new string('A', MaxLength + 1);
        var rule = new UserNameMaxLengthRule(name);

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenNameIsWellWithinLimit_ReturnsFalse()
    {
        // Arrange
        var rule = new UserNameMaxLengthRule("John Doe");

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeFalse();
    }

    [Fact]
    public void IsBroken_WhenNameIsSignificantlyOverLimit_ReturnsTrue()
    {
        // Arrange
        var name = new string('X', MaxLength + 100);
        var rule = new UserNameMaxLengthRule(name);

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeTrue();
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void ErrorCode_IsCorrectConstant()
    {
        // Arrange
        var rule = new UserNameMaxLengthRule(string.Empty);

        // Act & Assert
        rule.ErrorCode.Should().Be("USER_NAME_TOO_LONG");
    }

    [Fact]
    public void Message_ContainsMaxLength()
    {
        // Arrange
        var rule = new UserNameMaxLengthRule(string.Empty);

        // Act & Assert
        rule.Message.Should().Contain("255");
    }
}
