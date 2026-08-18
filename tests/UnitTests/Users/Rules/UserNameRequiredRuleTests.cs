using FluentAssertions;
using Raga.Domain.Users.Rules;
using Xunit;

namespace Raga.UnitTests.Users.Rules;

public sealed class UserNameRequiredRuleTests
{
    // ── IsBroken ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsBroken_WhenNameIsNull_ReturnsTrue()
    {
        // Arrange
        var rule = new UserNameRequiredRule(null!);

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeTrue();
    }

    [Fact]
    public void IsBroken_WhenNameIsEmpty_ReturnsTrue()
    {
        // Arrange
        var rule = new UserNameRequiredRule(string.Empty);

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeTrue();
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void IsBroken_WhenNameIsWhitespaceOnly_ReturnsTrue(string name)
    {
        // Arrange
        var rule = new UserNameRequiredRule(name);

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeTrue();
    }

    [Theory]
    [InlineData("John Doe")]
    [InlineData("A")]
    [InlineData(" Leading space")]
    public void IsBroken_WhenNameHasContent_ReturnsFalse(string name)
    {
        // Arrange
        var rule = new UserNameRequiredRule(name);

        // Act
        var broken = rule.IsBroken();

        // Assert
        broken.Should().BeFalse();
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void ErrorCode_IsCorrectConstant()
    {
        // Arrange
        var rule = new UserNameRequiredRule(string.Empty);

        // Act & Assert
        rule.ErrorCode.Should().Be("USER_NAME_REQUIRED");
    }

    [Fact]
    public void Message_IsHumanReadable()
    {
        // Arrange
        var rule = new UserNameRequiredRule(string.Empty);

        // Act & Assert
        rule.Message.Should().Be("Name is required.");
    }
}
